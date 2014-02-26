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
    /// <summary>
    /// Represents a battlefield scene
    /// This class contains everything to run the gameplay of the game
    /// 
    /// A world instance is created and two systems are added to it:
    ///     - HealthSystem that manages the health and shield
    ///     - PoisonSystem that manages the poison
    /// 
    /// A Mediator instance is used to communicate
    /// 
    /// ** Rules **
    /// In this scene you have to kill soldiers that are in a pending queue
    /// Every actions you will perform affect only the first soldier of the queue
    /// When the soldier die the next in the queue become the first
    /// If the queue is empty you have to add a soldier to the queue
    /// The number of soldier that can live in the queue at the same time is limited
    /// Following actions are available:
    ///     - Add a soldier to the queue (Enter for the keyboard or X for the gamepad)
    ///     - Do damage to the soldier (Space for the keyboard or A for the gamepad)
    ///     - Activate the shield of the soldier (Up for the keyboard and the gamepad)
    ///     - Disable the shield of the soldier (Down for the keyboard and the gamepad)
    ///     - Add poison to the soldier (Right for the keyboard and the gamepad)
    ///     - Remove poison from the soldier (Left for the keyboard and the gamepad)
    /// </summary>
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
        private const double TriggerDelay = 550.0d;

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

        /// <summary>
        /// Constructor of BattlefieldScene class
        /// </summary>
        public BattlefieldScene()
        {
            for (int i = 0; i < SimultaneousSoldier; i++)
            {
                GameObject soldier = new GameObject();
                soldier.Add(new HealthComponent());
                soldier.Add(new ShieldComponent());

                _gameObjectsPool.Push(soldier);
            }

            _mediator.RegisterListener(EventTypeConstant.DeathEvent, this); // Register a listener for a specific event
            _mediator.RegisterListener(EventTypeConstant.ShowMessageEvent, this);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads the entire scene
        /// </summary>
        public void Load()
        {
            HealthSystem healthSys = new HealthSystem(_mediator);
            PoisonSystem poisonSys = new PoisonSystem(_mediator);
            _myWorld.SystemManager.Add(healthSys); // Add the system to the world
            _myWorld.SystemManager.Add(poisonSys);

            _myWorld.SystemManager.Initialize(); // Initialize all systems
        }

        /// <summary>
        /// Updates the scene
        /// </summary>
        /// <param name="time">Elapsed time</param>
        public void Update(GameTime time)
        {
#if WINDOWS
            _previousKey = _currentKey;
            _currentKey = Keyboard.GetState();
#endif
            _previousPad = _currentPad;
            _currentPad = GamePad.GetState(PlayerIndex.One);

            CheckActions(time);

            _myWorld.Update(time); // Update the world

            _mediator.Tick(long.MaxValue); // Process the message queue
        }

        /// <summary>
        /// Checks action from the player input
        /// </summary>
        /// <param name="time">Elapsed time</param>
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

        /// <summary>
        /// Converts triggered input to action
        /// </summary>
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
                return;
            }

            if (IsActionTriggered(Buttons.DPadLeft, Keys.Left))
            {
                _currentAction = ActionEnum.PoisonOff;
                return;
            }

            if (IsActionTriggered(Buttons.DPadRight, Keys.Right))
            {
                _currentAction = ActionEnum.PoisonOn;
            }
        }

        /// <summary>
        /// Show a message to the output
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="args">Extra parameters</param>
        private void Writeline(string message, params object[] args)
        {
            Output.WriteLine(message, args);
        }

        #region Callbacks

        /// <summary>
        /// Called when a new message is send to this class
        /// </summary>
        /// <param name="msg">Message</param>
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

        /// <summary>
        /// Adds a new soldier to the queue
        /// </summary>
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

        /// <summary>
        /// Deals damage to the first soldier in the queue
        /// </summary>
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

        /// <summary>
        /// Switch the shield on or off
        /// </summary>
        /// <param name="activated">New state of the shield</param>
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

        /// <summary>
        /// Adds poison to the first soldier of the queue
        /// </summary>
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

        /// <summary>
        /// Removes poison from the first soldier of the queue
        /// </summary>
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

        /// <summary>
        /// Called when a soldier is dead
        /// </summary>
        /// <param name="info">Death information</param>
        private void OnDeath(DeathInfo info)
        {
            Writeline(SoldierDead, info.GameObjectId);

            GameObject deadSoldier = _myWorld.GameObjectManager.Get(info.GameObjectId);
            _myWorld.Remove(deadSoldier);
            _myWorld.Process();

            _gameObjectsPool.Push(deadSoldier);
            ResetSoldier(deadSoldier);

            _currentId++; // When adding a soldier Id are incremented, so the next one is the current id + 1
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Resets the soldier
        /// </summary>
        /// <param name="soldier">Soldier to reset</param>
        private void ResetSoldier(GameObject soldier)
        {
            soldier.Get<HealthComponent>().Life = HealthComponent.MaxLife;
            soldier.Get<ShieldComponent>().Power = ShieldComponent.MaxPower;
            soldier.Remove<PoisonComponent>();
        }

        /// <summary>
        /// Checks if there is a soldier in the queue
        /// </summary>
        /// <param name="soldier">First soldier in the queue if found otherwise null</param>
        /// <returns>Returns true if there is a soldier in the queue otherwise false</returns>
        private bool HasSoldierInQueue(out GameObject soldier)
        {
            soldier = _myWorld.GameObjectManager.Get(_currentId);

            return soldier != null;
        }

        #endregion

        #region Inputs

#if WINDOWS
        /// <summary>
        /// Checks if a key is pressed
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>Returns true if the key is pressed otherwise false</returns>
        private bool IsKeyPressed(Keys key)
        {
            return _previousKey.IsKeyDown(key) && _currentKey.IsKeyUp(key);
        }
#endif

        /// <summary>
        /// Checks if a button is pressed
        /// </summary>
        /// <param name="button">Button to check</param>
        /// <returns>Returns true if the button is pressed otherwise false</returns>
        private bool IsButtonPressed(Buttons button)
        {
            return _previousPad.IsButtonDown(button) && _currentPad.IsButtonUp(button);
        }

        /// <summary>
        /// Checks if a specified button or key is pressed
        /// </summary>
        /// <param name="button">Button to check</param>
        /// <param name="key">Key to check</param>
        /// <returns>Returns true if one is pressed otherwise false</returns>
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
