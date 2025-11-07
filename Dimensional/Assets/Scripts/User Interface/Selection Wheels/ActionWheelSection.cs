using Managers;
using Scriptables.Selection_Wheels;

namespace User_Interface.Selection_Wheels
{
    public class ActionWheelSection : WheelSection
    {
        private ActionWheelSectionDatum _actionWheelSectionDatum;

        public SelectionWheel ShapeSelectionWheel { get; private set; }

        public override void Initialize(WheelSectionDatum wheelSectionDatum, SelectionWheel selectionWheel)
        {
            base.Initialize(wheelSectionDatum, selectionWheel);
            _actionWheelSectionDatum = (ActionWheelSectionDatum)wheelSectionDatum;
            ShapeSelectionWheel =
                _actionWheelSectionDatum.SelectionWheelDatum.AttachSelectionWheel(UIManager.Instance.GetShapeSelectionWheelGameObject(_actionWheelSectionDatum.UseOtherMethod));
            ShapeSelectionWheel.GenerateSelectionWheel();
            ShapeSelectionWheel.Cancelled += SelectionWheelOnCancelled;
            SelectionWheel.Cancelled += ParentSelectionWheelOnCancelled;
        }

        protected override void OnHover()
        {
            ShapeSelectionWheel.Show();
        }

        protected override void OnStopHovering()
        {
            if (!ShapeSelectionWheel.IsActive) ShapeSelectionWheel.Hide();
        }

        public override void Select()
        {
            ShapeSelectionWheel.Show();
            if (_actionWheelSectionDatum.EnableShapeSelectionWheelOnSelect) ShapeSelectionWheel.Activate();
            base.Select();
        }
        
        private void ParentSelectionWheelOnCancelled(SelectionWheel selectionWheel)
        {
            ShapeSelectionWheel.Deactivate();
            if (_actionWheelSectionDatum.HideOnCancel) ShapeSelectionWheel.Hide();
        }
        
        private void SelectionWheelOnCancelled(SelectionWheel obj)
        {
            SelectionWheel.Activate();
            ShapeSelectionWheel.Deactivate();
            if (_actionWheelSectionDatum.HideOnCancel) ShapeSelectionWheel.Hide();
        }
    }
}
