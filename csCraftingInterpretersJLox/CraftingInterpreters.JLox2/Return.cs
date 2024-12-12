namespace CraftingInterpreters.JLox2
{
    public class Return : Exception
    {
        public object value;

        public Return(object value)
        {
            this.value = value;
        }
    }
}