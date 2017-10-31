namespace StyleFileCreator.App.Model
{
    public class DataItem
    {
        public string Name
        {
            get;
            private set;
        }

        public DataItem(string name)
        {
            Name = name;
        }
    }
}
