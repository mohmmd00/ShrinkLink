$(function () {
    // -----------------------------------------------------------------------
    // Weekly Sales - Bar Chart
    // -----------------------------------------------------------------------
    var chart = {
        series: [
            {
                name: "Sales",
                data: [4500, 4800, 4700, 5000, 5200, 5100, 5300],
            },
        ],
        chart: {
            type: "bar",
            height: 320,
            fontFamily: "inherit",
            foreColor: "#adb0bb",
            toolbar: { show: false },
        },
        colors: ["var(--bs-primary)"],
        plotOptions: {
            bar: {
                borderRadius: 5,
                columnWidth: "40%",
            },
        },
        dataLabels: { enabled: false },
        stroke: {
            width: 2,
            colors: ["transparent"],
        },
        xaxis: {
            categories: ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"],
            axisBorder: { show: false },
            axisTicks: { show: false },
        },
        yaxis: { tickAmount: 4 },
        grid: {
            borderColor: "rgba(0,0,0,0.1)",
            strokeDashArray: 3,
        },
        tooltip: { theme: "dark" },
    };

    var chart = new ApexCharts(document.querySelector("#bar-chart"), chart);
    chart.render();
});
