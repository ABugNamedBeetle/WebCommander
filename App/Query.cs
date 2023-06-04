namespace WebCommander.App;

public class Query
    {
        private string? label;
        private string? command;

    public Query(string label, string command)
    {
        this.Label = label;
        this.Command = command;
    }

    public string Label { get => label!; set => label = value; }
        public string Command { get => command!; set => command = value; }

    
    public override string? ToString()
    {
        return string.Format("Query:[ Label=\"{0}\", Command=\"{1}\" ]",Label, Command);;
    }
}

