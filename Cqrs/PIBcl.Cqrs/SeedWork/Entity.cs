using System;
using MediatR;
using System.Collections.Generic;
using Newtonsoft.Json;
using PIBcl.Core;

namespace PIBcl.Cqrs.SeedWork
{
    public abstract class Entity
    {
        int? _requestedHashCode;
        Guid _id;

        [JsonProperty(PropertyName = "id")]
        public virtual Guid Id {
            get {
                return _id;
            }
            set {
                _id = value;
            }
        }

        [JsonProperty(PropertyName = "owner")]
        public IUser Owner { get; private set; }

        [JsonProperty(PropertyName = "modifiedOn")]
        public DateTime ModifiedOn { get; private set; }

        [JsonProperty(PropertyName = "createdOn")]
        public DateTime CreatedOn { get; private set; }

        [JsonProperty(PropertyName = "createdBy")]
        public IUser CreatedBy { get; private set; }

        [JsonProperty(PropertyName = "modifiedBy")]
        public IUser ModifiedBy { get; private set; }

        [JsonProperty(PropertyName = "canal")]
        public CanalDeNegocio Canal { get; private set; }

        bool _isEnable;

        [JsonProperty(PropertyName = "isEnable")]
        public virtual bool IsEnable {
            get {
                return _isEnable;
            }
            set {
                _isEnable = value;
            }
        }

        [JsonProperty(PropertyName = "teste")]
        public string teste { get; set; }

        protected void SetCanalDeNegocio(CanalDeNegocio canal)
        {
            if (null == canal || string.IsNullOrEmpty(canal.Nome))
                throw new ArgumentNullException("O canal de negócio é obrigatório para criar uma entidade.");

            Canal = canal;
        }

        protected void SetCreateOn(DateTime createdOn)
        {
            if (createdOn <= DateTime.Now.Date)
                throw new ArgumentException("Data de Criação não pode ser menor que Data Atual", "CreatedOn");

            CreatedOn = createdOn;
            ModifiedOn = createdOn;
        }

        protected void SetModifiedOn(DateTime modifiedOn)
        {
            if (modifiedOn <= DateTime.Now.Date)
                throw new ArgumentException("Data de Modificação não pode ser menor que Data Atual", "ModifiedOn");

            ModifiedOn = modifiedOn;
        }

        public void SetCreatedBy(IUser user)
        {
            //if (null == user || user.Id == Guid.Empty)
            //    throw new ArgumentNullException("CreatedBy", "O usuário que está criando a entidade é obrigatório.");


            CreatedBy = user;
            ModifiedBy = user;
            Owner = user;
        }

        public void SetModifiedBy(IUser user)
        {
            //if (null == user || user.Id == Guid.Empty)
            //    throw new ArgumentNullException("ModifiedBy", "O ID do usuário que está modificando a entidade é obrigatório.");

            ModifiedBy = user;
            this.SetModifiedOn(DateTime.Now);
        }

        public void SetOwner(IUser user)
        {
            //if (null == user || user.Id == Guid.Empty)
            //    throw new ArgumentNullException("Owner", "O proprietário da entidade é obrigatório.");

            Owner = user;
        }

        private List<INotification> _domainEvents;
        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents?.AsReadOnly();

        public void AddDomainEvent(INotification eventItem)
        {
            _domainEvents = _domainEvents ?? new List<INotification>();
            _domainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(INotification eventItem)
        {
            _domainEvents?.Remove(eventItem);
        }

        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }

        public bool IsTransient()
        {
            return this.Id == default(Guid);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Entity))
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            if (this.GetType() != obj.GetType())
                return false;

            Entity item = (Entity)obj;

            if (item.IsTransient() || this.IsTransient())
                return false;
            else
                return item.Id == this.Id;
        }

        public override int GetHashCode()
        {
            if (!IsTransient())
            {
                if (!_requestedHashCode.HasValue)
                    _requestedHashCode = this.Id.GetHashCode() ^ 31;

                return _requestedHashCode.Value;
            }
            else
                return base.GetHashCode();

        }
        public static bool operator ==(Entity left, Entity right)
        {
            if (Object.Equals(left, null))
                return (Object.Equals(right, null)) ? true : false;
            else
                return left.Equals(right);
        }

        public static bool operator !=(Entity left, Entity right)
        {
            return !(left == right);
        }
    }
}
