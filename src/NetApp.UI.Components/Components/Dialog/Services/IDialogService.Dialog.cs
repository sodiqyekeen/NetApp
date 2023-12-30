namespace NetApp.UI.Components;

public partial interface IDialogService
{
    /// <summary>
    /// Shows a dialog with the component type as the body,
    /// passing the specified <paramref name="data"/> 
    /// </summary>
    /// <typeparam name="TData">Type of content to pass to component being displayed.</typeparam>
    /// <param name="dialogComponent">Type of component to display.</param>
    /// <param name="data">Content to pass to component being displayed.</param>
    /// <param name="parameters">Parameters to configure the dialog component.</param>
    Task<IDialogReference> ShowDialogAsync<TData>(Type dialogComponent, TData data, DialogParameters parameters)
        where TData : class;

    /// <summary>
    /// Shows a dialog with the component type as the body,
    /// passing the specified <paramref name="data"/> 
    /// </summary>
    /// <typeparam name="TDialog">Type of component to display.</typeparam>
    /// <param name="data">Content to pass to component being displayed.</param>
    /// <param name="parameters">Parameters to configure the dialog component.</param>
    Task<IDialogReference> ShowDialogAsync<TDialog>(object data, DialogParameters parameters)
         where TDialog : IDialogContent;

    /// <summary>
    /// Shows a dialog with the component type as the body.
    /// </summary>
    /// <typeparam name="TDialog">Type of component to display.</typeparam>
    /// <param name="parameters">Parameters to configure the dialog component.</param>
    Task<IDialogReference> ShowDialogAsync<TDialog>(DialogParameters parameters)
         where TDialog : IDialogContent;

    /// <summary>
    /// Updates a dialog.
    /// </summary>
    /// <typeparam name="TData">Type of content to pass to component being displayed.</typeparam>
    /// <param name="id">Id of the dialog to update.</param>
    /// <param name="parameters">Parameters to configure the dialog component.</param>
    Task<IDialogReference?> UpdateDialogAsync<TData>(string id, DialogParameters<TData> parameters)
        where TData : class;

    ///// <summary>
    ///// Shows a panel with the dialog component type as the body
    ///// </summary>
    ///// <typeparam name="TData">Type of content to pass to component being displayed.</typeparam>
    ///// <param name="dialogComponent">Type of component to display.</param>
    ///// <param name="data">Content to pass to component being displayed.</param>
    ///// <param name="parameters">Parameters to configure the dialog component.</param>
    //Task<IDialogReference> ShowPanelAsync<TData>(Type dialogComponent, TData data, DialogParameters parameters)
    //    where TData : class;

    ///// <summary>
    ///// Shows a panel with the dialog component type as the body
    ///// </summary>
    ///// <typeparam name="TDialog">Type of component to display.</typeparam>
    ///// <param name="data">Content to pass to component being displayed.</param>
    ///// <param name="parameters">Parameters to configure the dialog component.</param>
    //Task<IDialogReference> ShowPanelAsync<TDialog>(object data, DialogParameters parameters)
    //    where TDialog : IDialogContent;

    ///// <summary>
    ///// Shows a panel with the dialog component type as the body
    ///// </summary>
    ///// <typeparam name="TDialog">Type of component to display.</typeparam>
    ///// <param name="parameters">Parameters to configure the dialog component.</param>
    //Task<IDialogReference> ShowPanelAsync<TDialog>(DialogParameters parameters)
    //    where TDialog : IDialogContent;

}