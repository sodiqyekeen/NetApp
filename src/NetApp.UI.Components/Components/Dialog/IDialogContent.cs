namespace NetApp.UI.Components;


public interface IDialogContent
{
}

public interface IDialogContent<TContent> : IDialogContent
{
    TContent Content { get; set; }
}

