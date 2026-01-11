namespace Systems.Actions
{
    public class EmptyAction : Action
    {
        protected override void HandleActivation(ActionContext context) { }

        protected override void OnEntityChanged(ActionContext context) { }
    }
}
