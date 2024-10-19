using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;

namespace GantChartDemo.App
{
    public static class GenericCallback
    {

        public static DotNetObjectReference<GenericCallback<TParameter>> AsCallback<TParameter>(this Func<TParameter,Task> callbackFunction)
        {
            return DotNetObjectReference.Create(new GenericCallback<TParameter>(callbackFunction));
        }
    }
    public class GenericCallback<TParameter>
    {
        public GenericCallback(Func<TParameter,Task> handler)
        {
            Handler = handler;
        }

        public Func<TParameter, Task> Handler { get; }

        [JSInvokable]
        public async Task Raise(TParameter paramter)
        {
            await this.Handler(paramter);
        }

    }

    public class GantChartInterop
    {
        public GantChartInterop(IJSRuntime runtime, ElementReference container)
        {
            Runtime = runtime;
            Container = container;
        }

        public IJSRuntime Runtime { get; }
        public ElementReference Container { get; }

        public async Task RenderAsync(Func<int,Task> onBarClickedAsync)
        {
            var gantChartModule = await this.Runtime.InvokeAsync<IJSObjectReference>("import", "/GantChartInterop.js");
            long GetWebTimeFromString(string dateTime) => GetWebTime(DateTime.Parse(dateTime));

            long GetWebTime(DateTime dateTime) =>(long)dateTime.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;

            var optionsJson = $$"""
                            {
                    "series": [
                        {
                            "data": [
                                {
                                    "x": "Code",
                                    "y": [
                                        {{GetWebTimeFromString("2019-03-02")}},
                                        {{GetWebTimeFromString("2019-03-04")}}
                                    ]
                                },
                                {
                                    "x": "Test",
                                    "y": [
                                        {{GetWebTimeFromString("2019-03-04")}},
                                        {{GetWebTimeFromString("2019-03-08")}}
                                    ]
                                },
                                {
                                    "x": "Validation",
                                    "y": [
                                        {{GetWebTimeFromString("2019-03-08")}},
                                        {{GetWebTimeFromString("2019-03-12")}}
                                    ]
                                },
                                {
                                    "x": "Deployment",
                                    "y": [
                                        {{GetWebTimeFromString("2019-03-12")}},
                                        {{GetWebTimeFromString("2019-03-18")}}
                                    ]
                                }
                            ]
                        }
                    ],
                    "chart": {
                        "height": 350,
                        "type": "rangeBar"
                    },
                    "plotOptions": {
                        "bar": {
                            "horizontal": true
                        }
                    },
                    "xaxis": {
                        "type": "datetime"
                    }
                }
                """;
            var options = JsonDocument.Parse(optionsJson);
            var gantChart = await gantChartModule.InvokeAsync<IJSObjectReference>(
                "createGantChart", this.Container, options, onBarClickedAsync.AsCallback());
            await gantChart.InvokeVoidAsync("render");
        }
    }
}
