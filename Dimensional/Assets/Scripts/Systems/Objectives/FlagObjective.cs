using Scriptables.Objectives;
using Systems.Events;

namespace Systems.Objectives
{
    public class FlagObjective : Objective
    {
        private FlagObjectiveDatum _flagObjectiveDatum;

        public override void Initialize(ObjectiveDatum objectiveDatum)
        {
            base.Initialize(objectiveDatum);
            _flagObjectiveDatum = objectiveDatum as FlagObjectiveDatum;
        }

        public override void TryProgress(GameEvent gameEvent)
        {
            if (gameEvent is FlagEvent flagEvent && flagEvent.FlagId == _flagObjectiveDatum.FlagId)
            {
                SetIsCompleted(true);
            }
        }
    }
}
