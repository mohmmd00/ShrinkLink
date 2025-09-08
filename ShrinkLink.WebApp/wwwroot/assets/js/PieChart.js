$(function () {
    // -----------------------------------------------------------------------
    // browser diversity (Pie Chart)
    // -----------------------------------------------------------------------

    var chart = {
        series: [44, 55, 13], // Example values
        chart: {
            type: "pie",
            height: 320,
            fontFamily: "inherit",
            foreColor: "#adb0bb",
            toolbar: {
                show: false,
            },
        },
        labels: ["Direct", "Referral", "Social"], // Example labels
        colors: ["var(--bs-gray-300)", "var(--bs-primary)", "var(--bs-success)"],
        dataLabels: {
            enabled: true,
        },
        legend: {
            show: true,
            position: "bottom",
        },
        tooltip: {
            theme: "dark",
        },
    };

    var chart = new ApexCharts(document.querySelector("#PieChart"),chart);
    chart.render();
});