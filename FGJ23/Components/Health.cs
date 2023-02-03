using FGJ23.Support;

namespace FGJ23.Components
{
    internal class Health : Component, ILoggable
    {

        [Loggable]
        public int Maximum;
        [Loggable]
        public int Current;
        public bool DestroyOnNegative = true;

        public Health(int max)
        {
            this.Maximum = this.Current = max;
        }

        public int Hit(int damage)
        {
            this.Current -= damage;
            if (this.Current <= 0 && this.DestroyOnNegative)
            {
                Entity.Destroy();
            }
            return this.Current;
        }
    }
}
