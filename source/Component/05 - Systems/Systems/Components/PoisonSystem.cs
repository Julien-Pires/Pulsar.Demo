using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Pulsar;
using Pulsar.Components;

namespace Systems.Components
{
    /// <summary>
    /// Manages all the poison component
    /// </summary>
    public sealed class PoisonSystem : Pulsar.Components.System
    {
        #region Fields

        private const string PoisonDamage = "The poison deals {0} damage";

        private readonly Mediator _mediator;
        private readonly Random _rand = new Random();
        private readonly Type[] _componentTypes = {typeof(PoisonComponent)};
        private readonly List<PoisonComponent> _components = new List<PoisonComponent>();

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of PoisonSystem class
        /// </summary>
        /// <param name="mediator">Mediator used to communicate</param>
        public PoisonSystem(Mediator mediator)
        {
            _mediator = mediator;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the system
        /// </summary>
        /// <param name="time">Elapsed time</param>
        public override void Update(GameTime time)
        {
            for (int i = 0; i < _components.Count; i++)
            {
                PoisonComponent poison = _components[i];
                double lastDamageTime = poison.LastDamage + time.ElapsedGameTime.TotalMilliseconds;
                poison.LastDamage = Math.Min(lastDamageTime, poison.Frequency);

                if (poison.LastDamage >= poison.Frequency)
                {
                    int damage = _rand.Next(poison.MinDamage, poison.MaxDamage);
                    ShowMessage(PoisonDamage, damage);
                    SendDamage(poison.Parent.Id, damage);
                    poison.LastDamage = 0.0d;
                }
            }
        }

        /// <summary>
        /// Sends a message that indicates the poison deals damage
        /// </summary>
        /// <param name="id">Game object id</param>
        /// <param name="damage">Quantity of damage</param>
        private void SendDamage(int id, int damage)
        {
            DamageInfo damageInfo = new DamageInfo(id, damage);
            Message msg = new Message(EventTypeConstant.DamageEvent, damageInfo);
            _mediator.Queue(msg);
        }

        /// <summary>
        /// Sends a message that indicates something must be shown on the screen
        /// </summary>
        /// <param name="message">Message to show</param>
        /// <param name="args">Extra parameters</param>
        private void ShowMessage(string message, params object[] args)
        {
            MessageInfo info = new MessageInfo(message, args);
            Message msg = new Message(EventTypeConstant.ShowMessageEvent, info);
            _mediator.Queue(msg);
        }

        #region Callbacks

        public override void Register(Component compo)
        {
            PoisonComponent poison = compo as PoisonComponent;
            if(poison == null)
                throw new ArgumentException();

            _components.Add(poison);
        }

        public override bool Unregister(Component compo)
        {
            PoisonComponent poison = compo as PoisonComponent;
            if(poison == null)
                throw new ArgumentException();

            return _components.Remove(poison);
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
