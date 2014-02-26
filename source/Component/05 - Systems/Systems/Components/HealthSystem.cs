using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Pulsar;
using Pulsar.Components;

namespace Systems.Components
{
    /// <summary>
    /// Manages the health and shield component
    /// </summary>
    public sealed class HealthSystem : Pulsar.Components.System, IEventHandler
    {
        #region Nested

        /// <summary>
        /// Associates health and shield together
        /// </summary>
        private sealed class HealthInfo
        {
            #region Constructor

            /// <summary>
            /// Constructor of HealthInfo class
            /// </summary>
            /// <param name="id">Game object id</param>
            public HealthInfo(int id)
            {
                GameObjectId = id;
                Dead = true;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets the game object id
            /// </summary>
            public int GameObjectId { get; private set; }

            /// <summary>
            /// Gets or sets a value that indicates if the game object is dead
            /// </summary>
            public bool Dead { get; set; }

            /// <summary>
            /// Gets or sets the health component
            /// </summary>
            public HealthComponent Health { get; set; }

            /// <summary>
            /// Gets or sets the shield component
            /// </summary>
            public ShieldComponent Shield { get; set; }

            /// <summary>
            /// Gets or sets a value that indicates the last time damages have been done to the game object
            /// </summary>
            internal double LastDamageTimer { get; set; }

            #endregion
        }

        #endregion

        #region Fields

        private const string DamageReceived = "The soldier {0} received {1} damage";
        private const string ShieldAbsorbed = "The shield has absorbed {0} damage, {1} power remaining";
        private const string LifeLost = "{0} point of life lost, {1} point of life remaining";
        private const string ShieldRegenerated = "The shield of soldier {0} has regenerated {1} power";

        private readonly Type[] _componentTypes = {typeof(HealthComponent), typeof(ShieldComponent)};
        private readonly Dictionary<int, HealthInfo> _healthMap = new Dictionary<int, HealthInfo>();
        private readonly Mediator _mediator;
        private int _aliveCount;
        private int _deadCount;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of HealthSystem class
        /// </summary>
        /// <param name="mediator">Mediator used to communicate</param>
        public HealthSystem(Mediator mediator)
        {
            _mediator = mediator;

            /* 
             * Register this class to listen for all DamageEvent
             * This Event occurres each time a game object will get hurt
             */
            _mediator.RegisterListener(EventTypeConstant.DamageEvent, this);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the system
        /// </summary>
        /// <param name="time">Elapsed time</param>
        public override void Update(GameTime time)
        {
            foreach (HealthInfo info in _healthMap.Values)
            {
                if(info.Dead)
                    return;

                double delta = time.ElapsedGameTime.TotalMilliseconds;
                double value = info.LastDamageTimer + delta;
                info.LastDamageTimer = Math.Min(value, double.MaxValue);

                if (info.Shield != null)
                {
                    UpdateShieldTimer(info, delta);
                    RegenerateShield(info);
                }
            }
        }

        #region Callback

        /// <summary>
        /// Called when the mediator send a message to this class
        /// </summary>
        /// <param name="msg">Message</param>
        public void HandleEvent(Message msg)
        {
            if (msg.Event == EventTypeConstant.DamageEvent)
            {
                DamageInfo info = msg.Payload as DamageInfo;
                if(info == null)
                    throw new Exception();

                DoDamage(info.GameObjectId, info.Damage);
            }
        }

        /// <summary>
        /// Called when a new component is added to the world
        /// </summary>
        /// <param name="compo">Component</param>
        public override void Register(Component compo)
        {
            if(compo == null)
                throw new ArgumentNullException("compo");

            Type componentType = compo.GetType();
            if (componentType == typeof (HealthComponent))
                ActivateHealth(compo as HealthComponent);
            else if (componentType == typeof (ShieldComponent))
                ActivateShield(compo as ShieldComponent);
        }

        /// <summary>
        /// Called when a component is removed from the world
        /// </summary>
        /// <param name="compo">Component</param>
        /// <returns>Returns true if successfully removed otherwise false</returns>
        public override bool Unregister(Component compo)
        {
            if (compo == null)
                return false;

            Type componentType = compo.GetType();
            if (componentType == typeof(HealthComponent))
                RemoveHealth(compo as HealthComponent);
            else if (componentType == typeof(ShieldComponent))
                RemoveShield(compo as ShieldComponent);

            return true;
        }

        #endregion

        #region Components callback

        /// <summary>
        /// Activates the health and makes a game object alive
        /// </summary>
        /// <param name="health">Health</param>
        private void ActivateHealth(HealthComponent health)
        {
            HealthInfo info = EnsureHealthInfo(health.Parent.Id);
            info.Dead = false;
            info.Health = health;

            _aliveCount++;
        }

        /// <summary>
        /// Removes the health and makes a game object dead
        /// </summary>
        /// <param name="health"></param>
        private void RemoveHealth(HealthComponent health)
        {
            int id = health.Parent.Id;
            HealthInfo info;
            if (!_healthMap.TryGetValue(id, out info))
                return;

            _healthMap.Remove(id);

            if (info.Dead)
            {
                _aliveCount--;
                _deadCount++;
            }
        }

        /// <summary>
        /// Adds a shield to a game object
        /// </summary>
        /// <param name="shield">Shield</param>
        private void ActivateShield(ShieldComponent shield)
        {
            HealthInfo info = EnsureHealthInfo(shield.Parent.Id); ;
            info.Shield = shield;
        }

        /// <summary>
        /// Removes a shield from a game object
        /// </summary>
        /// <param name="shield">Shield</param>
        private void RemoveShield(ShieldComponent shield)
        {
            HealthInfo info;
            if (!_healthMap.TryGetValue(shield.Parent.Id, out info))
                return;

            info.Shield = null;
        }

        #endregion

        #region Actions

        /// <summary>
        /// Updates regeneration timer
        /// </summary>
        /// <param name="info">Shield info to update</param>
        /// <param name="delta">Time in ms since the last update</param>
        private void UpdateShieldTimer(HealthInfo info, double delta)
        {
            ShieldComponent shield = info.Shield;
            double value = shield.LastRegeneration + delta;
            shield.LastRegeneration = Math.Min(value, shield.RegenerationFrequency);
        }

        /// <summary>
        /// Regenerates the shield
        /// </summary>
        /// <param name="info">Shield info</param>
        private void RegenerateShield(HealthInfo info)
        {
            ShieldComponent shield = info.Shield;
            if (!CanRegenerate(info.Shield)) return;

            if ((shield.LastRegeneration >= shield.RegenerationFrequency) && (info.LastDamageTimer >= shield.RegenerationDelay))
            {
                shield.Power += shield.Regeneration;
                shield.LastRegeneration = 0.0d;

                ShowMessage(ShieldRegenerated, info.GameObjectId, shield.Regeneration);
            }
        }

        /// <summary>
        /// Deals damage to a game object
        /// </summary>
        /// <param name="id">Game object id</param>
        /// <param name="damage">Quantity of damage</param>
        private void DoDamage(int id, int damage)
        {
            HealthInfo info;
            if(!_healthMap.TryGetValue(id, out info))
                return;

            if(info.Dead)
                return;

            ShowMessage(DamageReceived, id, damage);

            int remainingDamage = damage;
            ShieldComponent shield = info.Shield;
            if ((shield != null) && (shield.Activated))
            {
                int shieldDiff = shield.Power - damage;
                shield.Power = Math.Max(shieldDiff, ShieldComponent.MinPower);
                remainingDamage = (shieldDiff < 0) ? Math.Abs(shieldDiff) : 0;

                ShowMessage(ShieldAbsorbed, (damage - remainingDamage), shield.Power);
            }

            HealthComponent health = info.Health;
            int lifeDiff = health.Life - remainingDamage;
            health.Life = Math.Max(lifeDiff, HealthComponent.MinLife);

            info.LastDamageTimer = 0.0d;

            ShowMessage(LifeLost, remainingDamage, health.Life);

            if (health.Life <= HealthComponent.MinLife)
            {
                info.Dead = true;
                SendDeathMessage(id);
            }

            ShowMessage("\n");
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Sends a message to indicate that a game object is dead
        /// </summary>
        /// <param name="id">Game object id</param>
        private void SendDeathMessage(int id)
        {
            DeathInfo info = new DeathInfo(id);
            Message msg = new Message(EventTypeConstant.DeathEvent, info);
            _mediator.Queue(msg);
        }

        /// <summary>
        /// Sends a message to indicate that something must be show on the screen
        /// </summary>
        /// <param name="message">Message to show</param>
        /// <param name="args">Extra parameters</param>
        private void ShowMessage(string message, params object[] args)
        {
            MessageInfo info = new MessageInfo(message, args);
            Message msg = new Message(EventTypeConstant.ShowMessageEvent, info);
            _mediator.Queue(msg);
        }

        /// <summary>
        /// Checks if a shield can be regenerated
        /// </summary>
        /// <param name="shield">Shield to check</param>
        /// <returns>Returns true if it can regenerate otherwise false</returns>
        private bool CanRegenerate(ShieldComponent shield)
        {
            return (shield.Activated) && (shield.Power < ShieldComponent.MaxPower);
        }

        /// <summary>
        /// Ensures that a HealthInfo instance exists
        /// </summary>
        /// <param name="id">Game object id</param>
        /// <returns>Returns a HealthInfo instance</returns>
        private HealthInfo EnsureHealthInfo(int id)
        {
            HealthInfo info;
            if (!_healthMap.TryGetValue(id, out info))
            {
                info = new HealthInfo(id);
                _healthMap.Add(id, info);
            }

            return info;
        }

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// Gets the associated component type
        /// </summary>
        public override Type[] ComponentTypes
        {
            get { return _componentTypes; }
        }

        #endregion
    }
}
