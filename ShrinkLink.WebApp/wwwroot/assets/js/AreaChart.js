$(function () {
    var chart = {
        series: [
            {
                name: "Engagement",
                data: [10, 41, 35, 51, 49, 62, 69],
            },
        ],
        chart: {
            type: "area",
            height: 320,
            fontFamily: "inherit",
            foreColor: "#adb0bb",
            toolbar: { show: false },
        },
        colors: ["var(--bs-primary)"],
        stroke: {
            curve: "smooth",
            width: 2,
        },
        dataLabels: { enabled: false },
        grid: {
            borderColor: "rgba(0,0,0,0.1)",
            strokeDashArray: 3,
        },
        xaxis: {
            categories: ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"],
            axisBorder: { show: false },
            axisTicks: { show: false },
        },
        yaxis: { tickAmount: 4 },
        tooltip: { theme: "dark" },
    };

    var chart = new ApexCharts(document.querySelector("#chart-area"), chart);
    chart.render();
});
