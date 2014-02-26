using System;
using System.Collections.Generic;
using Systems.Components;

using Common;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Pulsar;
using Pulsar.Components;

namespace Systems
{
    public sealed class BattlefieldScene : IEventHandler
    {
        #region Fields

        private const string SoldierBirth = "The soldier {0} is added to the queue...";
        private const string SoldierDead = "The soldier {0} is dead...";
        private const string ShieldOn = "Shield activated";
        private const string ShieldOff = "Shield disabled";
        private const string SoldierPoisoned = "The soldier {0} has been poisoned";
        private const string SoldierNotPoisoned = "The poison has been removed of the soldier {0}";
        private const string EmptyQueue = "No soldiers in the queue, create one first";
        private const string AllSoldierUsed = "The queue is full, kill a soldier before trying to add new one";

        private const int SimultaneousSoldier = 10;
        private const int Damage = 10;
        private const double TriggerDelay = 650.0d;

#if WINDOWS
        private KeyboardState _previousKey;
        private KeyboardState _currentKey;
#endif
        private GamePadState _previousPad;
        private GamePadState _currentPad;

        private readonly World _myWorld = new World();
        private readonly Mediator _mediator = new Mediator();
        private readonly Stack<GameObject> _gameObjectsPool = new Stack<GameObject>(SimultaneousSoldier);
        private int _nextId;
        private int _currentId;
        private double _lastTrigger;
        private ActionEnum _currentAction = ActionEnum.None;

        #endregion

        #region Constructor

        public BattlefieldScene()
        {
            for (int i = 0; i < SimultaneousSoldier; i++)
            {
                GameObject soldier = new GameObject();
                soldier.Add(new HealthComponent());
                soldier.Add(new ShieldComponent());

                _gameObjectsPool.Push(soldier);
            }

            _mediator.RegisterListener(EventTypeConstant.DeathEvent, this);
            _mediator.RegisterListener(EventTypeConstant.ShowMessageEvent, this);
        }

        #endregion

        #region Methods

        public void Load()
        {
            HealthSystem healthSys = new HealthSystem(_mediator);
            PoisonSystem poisonSys = new PoisonSystem(_mediator);
            _myWorld.SystemManager.Add(healthSys);
            _myWorld.SystemManager.Add(poisonSys);

            _myWorld.SystemManager.Initialize();
        }

        public void Update(GameTime time)
        {
#if WINDOWS
            _previousKey = _currentKey;
            _currentKey = Keyboard.GetState();
#endif
            _previousPad = _currentPad;
            _currentPad = GamePad.GetState(PlayerIndex.One);

            CheckActions(time);

            _myWorld.Update(time);

            _mediator.Tick(long.MaxValue);
        }

        private void CheckActions(GameTime time)
        {
            _lastTrigger += time.ElapsedGameTime.TotalMilliseconds;
            _lastTrigger = Math.Min(_lastTrigger, TriggerDelay);
            if (_lastTrigger < TriggerDelay)
                return;

            InputToAction();
            bool triggered = _currentAction != ActionEnum.None;
            if (!triggered) return;

            switch (_currentAction)
            {
                case ActionEnum.CreateSoldier:
                    CreateSoldier();
                    break;
                case ActionEnum.HitSoldier:
                    HitSoldier();
                    break;
                case ActionEnum.ShieldOn:
                    SwitchShield(true);
                    break;
                case ActionEnum.ShieldOff:
                    SwitchShield(false);
                    break;
                case ActionEnum.PoisonOn:
                    AddPoison();
                    break;
                case ActionEnum.PoisonOff:
                    RemovePoison();
                    break;
            }

            _lastTrigger = 0.0d;
            _currentAction = ActionEnum.None;
        }

        private void InputToAction()
        {
            if (IsActionTriggered(Buttons.X, Keys.Enter))
            {
                _currentAction = ActionEnum.CreateSoldier;
                return;
            }

            if (IsActionTriggered(Buttons.A, Keys.Space))
            {
                _currentAction = ActionEnum.HitSoldier;
                return;
            }

            if (IsActionTriggered(Buttons.DPadUp, Keys.Up))
            {
                _currentAction = ActionEnum.ShieldOn;
                return;
            }

            if (IsActionTriggered(Buttons.DPadDown, Keys.Down))
            {
                _currentAction = ActionEnum.ShieldOff;
            }

            if (IsActionTriggered(Buttons.DPadLeft, Keys.Left))
            {
                _currentAction = ActionEnum.PoisonOff;
            }

            if (IsActionTriggered(Buttons.DPadRight, Keys.Right))
            {
                _currentAction = ActionEnum.PoisonOn;
            }
        }

        private void Writeline(string message, params object[] args)
        {
            Output.WriteLine(message, args);
        }

        #region Callbacks

        public void HandleEvent(Message msg)
        {
            if (msg.Event == EventTypeConstant.DeathEvent)
            {
                DeathInfo info = msg.Payload as DeathInfo;
                if(info == null)
                    throw new Exception();

                OnDeath(info);
                return;
            }

            if (msg.Event == EventTypeConstant.ShowMessageEvent)
            {
                MessageInfo info = msg.Payload as MessageInfo;
                if(info == null)
                    throw new Exception();

                Writeline(info.Message, info.Arguments);
            }
        }

        #endregion

        #region Actions

        private void CreateSoldier()
        {
            if (_gameObjectsPool.Count == 0)
            {
                Writeline(AllSoldierUsed);

                return;
            }

            GameObject soldier = _gameObjectsPool.Pop();
            soldier.Id = _nextId++;

            _myWorld.Add(soldier);
            _myWorld.Process();

            Writeline(SoldierBirth, soldier.Id);
        }

        private void HitSoldier()
        {
            GameObject soldier;
            if (!HasSoldierInQueue(out soldier))
            {
                Writeline(EmptyQueue);

                return;
            }

            DamageInfo dmgInfo = new DamageInfo(soldier.Id, Damage);
            Message msg = new Message(EventTypeConstant.DamageEvent, dmgInfo);
            _mediator.Queue(msg);
        }

        private void SwitchShield(bool activated)
        {
            GameObject soldier;
            if (!HasSoldierInQueue(out soldier))
            {
                Writeline(EmptyQueue);

                return;
            }

            soldier.Get<ShieldComponent>().Activated = activated;

            Writeline(activated ? ShieldOn : ShieldOff);
        }

        private void AddPoison()
        {
            GameObject soldier;
            if (!HasSoldierInQueue(out soldier))
            {
                Writeline(EmptyQueue);

                return;
            }

            PoisonComponent poison = soldier.Get<PoisonComponent>();
            if(poison != null)
                return;

            poison = new PoisonComponent();
            soldier.Add(poison);

            Writeline(SoldierPoisoned, _currentId);
        }

        public void RemovePoison()
        {
            GameObject soldier;
            if (!HasSoldierInQueue(out soldier))
            {
                Writeline(EmptyQueue);

                return;
            }

            soldier.Remove<PoisonComponent>();

            Writeline(SoldierNotPoisoned, _currentId);
        }

        private void OnDeath(DeathInfo info)
        {
            Writeline(SoldierDead, info.GameObjectId);

            GameObject deadSoldier = _myWorld.GameObjectManager.Get(info.GameObjectId);
            _myWorld.Remove(deadSoldier);
            _myWorld.Process();

            _gameObjectsPool.Push(deadSoldier);
            ResetSoldier(deadSoldier);

            _currentId++;
        }

        #endregion

        #region Helpers

        private void ResetSoldier(GameObject soldier)
        {
            soldier.Get<HealthComponent>().Life = HealthComponent.MaxLife;
            soldier.Get<ShieldComponent>().Power = ShieldComponent.MaxPower;
            soldier.Remove<PoisonComponent>();
        }

        private bool HasSoldierInQueue(out GameObject soldier)
        {
            soldier = _myWorld.GameObjectManager.Get(_currentId);

            return soldier != null;
        }

        #endregion

        #region Inputs

#if WINDOWS
        private bool IsKeyPressed(Keys key)
        {
            return _previousKey.IsKeyDown(key) && _currentKey.IsKeyUp(key);
        }
#endif

        private bool IsButtonPressed(Buttons button)
        {
            return _previousPad.IsButtonDown(button) && _currentPad.IsButtonUp(button);
        }

        private bool IsActionTriggered(Buttons button, Keys key)
        {
#if WINDOWS
            if (IsButtonPressed(button) || IsKeyPressed(key))
#else
            if (IsButtonPressed(button))
#endif
                return true;

            return false;
        }

        #endregion

        #endregion
    }
}
