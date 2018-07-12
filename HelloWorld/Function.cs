using Amazon.Lambda.Core;

namespace HelloWorld
{
    public class Function
    {
        public class FunctionInput
        {
            public string Name { get; set; }
        }

        /// <summary>
        /// Return a greeting.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public string FunctionHandler(FunctionInput input, ILambdaContext context)
        {
            return $"Hello, {input.Name}!";
        }
    }
}
