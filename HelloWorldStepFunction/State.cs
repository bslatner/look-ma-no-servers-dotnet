using System;
using System.Collections.Generic;
using System.Text;

namespace HelloWorldStepFunction
{
    /// <summary>
    /// The state passed between the step function executions.
    /// </summary>
    public class State
    {
        /// <summary>
        /// Input value when starting the execution
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Output value.
        /// </summary>
        public string Greeting { get; set; }
    }
}
