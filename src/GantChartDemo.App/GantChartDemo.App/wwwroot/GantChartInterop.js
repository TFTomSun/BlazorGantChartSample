import { default as AppexCharts } from  "./packages/apexcharts/dist/apexcharts.esm.js"
export function createGantChart(element, options, callback) {

    // attach events
    options.chart.events = {
        click: (mouseEvent,chartContext,options)=> {
            callback.invokeMethodAsync("Raise", options.dataPointIndex);
        },
    }

    // create chart
    const chart = new AppexCharts(element, options);
    
    
    return chart;
}