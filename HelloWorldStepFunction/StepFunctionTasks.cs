using Amazon.Lambda.Core;


namespace HelloWorldStepFunction
{
    public class StepFunctionTasks
    {
        public State EnglishGreeting(State state, ILambdaContext context)
        {
            state.Greeting = string.IsNullOrEmpty(state.Name)
                ? "Hello!"
                : $"Hello, {state.Name}!";

            return state;
        }
        
        public State SpanishGreeting(State state, ILambdaContext context)
        {
            state.Greeting = string.IsNullOrEmpty(state.Name)
                ? "Hola!"
                : $"Hola, {state.Name}!";

            return state;
        }
    }
}
