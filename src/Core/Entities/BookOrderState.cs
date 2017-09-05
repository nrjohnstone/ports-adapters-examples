using System;

namespace Core.Entities
{
    public class BookOrderState
    {
        private readonly string _state;

        protected BookOrderState(string state)
        {
            if (string.IsNullOrWhiteSpace(state))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(state));
            _state = state;
        }

        public override bool Equals(object other)
        {
            if (other == null)
                return false;

            if (other is BookOrderState == false)
                return false;

            return string.Equals(_state.ToString(), other.ToString());
        }

        public override int GetHashCode()
        {
            return (_state != null ? _state.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return _state;
        }

        public static BookOrderState Pending => new BookOrderState("Pending");
    }
}