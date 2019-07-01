namespace RP_Shared_Script
{
    /// <summary>
    /// A delegate for event handlers with a generic sender type.
    /// </summary>
    /// <typeparam name="TSender">Type of the class that invoked the <see cref="GenericEventHandler{TSender,TArgs}"/>.</typeparam>
    /// <typeparam name="TArgs">Type of the arguments object.</typeparam>
    /// <param name="sender">The object that invoked the  the <see cref="GenericEventHandler{TSender,TArgs}"/>.</param>
    /// <param name="args">Arguments object of the event.</param>
    public delegate void GenericEventHandler<in TSender, in TArgs>(TSender sender, TArgs args) where TSender : class;

    /// <summary>
    /// A delegate for event handlers with a generic sender type.
    /// </summary>
    /// <typeparam name="TSender">Type of the class that invoked the <see cref="GenericEventHandler{TSender,TArgs}"/>.</typeparam>
    /// <param name="sender">The object that invoked the  the <see cref="GenericEventHandler{TSender,TArgs}"/>.</param>
    public delegate void GenericEventHandler<in TSender>(TSender sender) where TSender : class;
}
