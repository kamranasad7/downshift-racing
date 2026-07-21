namespace Downshift
{
    public enum RunState { Descending, Crashed, BlownUp, Results }

    public class RunMachine
    {
        public RunState State { get; private set; } = RunState.Descending;

        public bool TryCrash()
        {
            if (State != RunState.Descending) return false;
            State = RunState.Crashed;
            return true;
        }

        public bool TryBlowUp()
        {
            if (State != RunState.Descending) return false;
            State = RunState.BlownUp;
            return true;
        }

        public bool ToResults()
        {
            if (State != RunState.Crashed && State != RunState.BlownUp) return false;
            State = RunState.Results;
            return true;
        }

        public void Reset() => State = RunState.Descending;
    }
}
