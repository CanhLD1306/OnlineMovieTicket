$(document).ready(function () {
    loadOverviewStatistics();
    LoadRevenueChart('month');
    LoadSeatTypeChart();

    $('#timeGrouping').change(function() {
        var selectedPeriod = $(this).val();
        LoadRevenueChart(selectedPeriod);
    });

    $('#moviesTable').DataTable({
        processing: true,
        serverSide: true,
        paging: false,
        searching: false,
        ordering: false,
        lengthMenu: [5,10,25,50],
        ajax: {
            url: urlGetTop5Movies,
            type: 'POST',
            data: function (d) {
                return {
                    Draw: d.draw,
                    PageNumber: (d.start / d.length) + 1,
                    PageSize: d.length,
                };
            }
        },  
        columns: [
            { 
                "data": 'posterURL',
                "className": "text-center",
                "render": function (data, type, row) {
                    return `<img src="${data}" alt="Poster" style="width: 60px; height: auto;" />`;
                }
            },
            { "data": 'title'},
            {
                "data": 'totalRevenue',
                "className": "text-center",
                "render": $.fn.dataTable.render.number(',', '.', 2, '$')
            },
            { 
                "data": 'totalTicketSold',
                "className": "text-center"
            },
            { "data": 'releaseDate', 
                className: "text-center",
                "render": function (data, type, row) {
                    const date = new Date(data);
                    return date.toLocaleDateString('vi-VN');
                }
            },
        ],
        language: {
            emptyTable: "No data available in table",
            infoFiltered: ""
        }
    });
});

function loadOverviewStatistics () {
    $.ajax({
        url: urlGetOverviewStatistics,
        type: 'GET',
        success: function (response) {
            $('#overviewStatisticsCards').html(response);
        },
        error: function (xhr, status, error) {
            toastr.error('There was an error processing your request: ' + error);
        }
    });
}

function LoadRevenueChart(groupBy) {
    $.ajax({
        url: urlGetRevenueByDateGroup,
        type: 'GET',
        data: { groupBy: groupBy },
        success: function (data) {
            if (window.revenueChart && typeof window.revenueChart.destroy === 'function') {
                window.revenueChart.destroy();
            }
            const labels = data.map(item => item.label);
            const revenues = data.map(item => item.totalRevenue);
            const ctx = document.getElementById('revenueChart').getContext('2d');

            window.revenueChart = new Chart(ctx, {
                type: 'line',
                data: {
                    labels: labels,
                    datasets: [{
                        label: 'Total Revenue ($)',
                        data: revenues,
                        backgroundColor: 'rgba(78, 115, 223, 0.05)',
                        borderColor: 'rgba(78, 115, 223, 1)',
                        pointRadius: 3,
                        pointBackgroundColor: 'rgba(78, 115, 223, 1)',
                        pointBorderColor: 'rgba(78, 115, 223, 1)',
                        pointHoverRadius: 3,
                        pointHoverBackgroundColor: 'rgba(78, 115, 223, 1)',
                        pointHoverBorderColor: 'rgba(78, 115, 223, 1)',
                        pointHitRadius: 10,
                        pointBorderWidth: 2,
                        tension: 0.4
                    }]
                },
                options: {
                    maintainAspectRatio: false,
                    layout: {
                        padding: { left: 10, right: 25, top: 25, bottom: 0 }
                    },
                    scales: {
                        x: {
                            time: { unit: 'date' },
                            grid: { display: false, drawBorder: false },
                            ticks: { maxTicksLimit: 7 }
                        },
                        y: {
                            ticks: {
                                maxTicksLimit: 5,
                                padding: 10,
                                callback: function (value) { return '$' + value; }
                            },
                            grid: {
                                color: "rgb(234, 236, 244)",
                                zeroLineColor: "rgb(234, 236, 244)",
                                drawBorder: false,
                                borderDash: [2],
                                zeroLineBorderDash: [2]
                            }
                        }
                    },
                    plugins: {
                        legend: {
                            display: true,
                            onClick: function (e) {
                                e.stopPropagation();
                            }
                        },
                        tooltip: {
                            backgroundColor: "rgb(255,255,255)",
                            bodyColor: "#858796",
                            titleMarginBottom: 10,
                            titleColor: '#6e707e',
                            titleFont: { size: 14 },
                            borderColor: '#dddfeb',
                            borderWidth: 1
                        }
                    },
                    interaction: {
                        mode: 'nearest',
                        intersect: false
                    }
                }
            });
        },
        error: function () {
            console.error('Failed to load revenue data');
        }
    });
}

function LoadSeatTypeChart() {
    $.ajax({
        url: urlGetTicketRatioBySeatType,
        type: 'GET',
        success: function (data) {
            if (window.seatTypeChart && typeof window.seatTypeChart.destroy === 'function') {
                window.seatTypeChart.destroy();
            }

            console.log("SeatType:", data);

            const labels = data.map(item => item.label);
            const values = data.map(item => item.value);
            const colors = data.map(item => item.color);

            const ctx = document.getElementById('seatTypeChart').getContext('2d');

            window.seatTypeChart = new Chart(ctx, {
                type: 'pie',
                data: {
                    labels: labels,
                    datasets: [{
                        data: values,
                        backgroundColor: colors,
                        borderWidth: 1
                    }]
                },
                options: {
                    maintainAspectRatio: false,
                    plugins: {
                        legend: {
                            display: true,
                            position: 'bottom',
                            onClick: function (e) {
                                e.stopPropagation();
                            }
                        },
                        tooltip: {
                            callbacks: {
                                label: function (tooltipItem) {
                                    const label = tooltipItem.label || '';
                                    const value = tooltipItem.raw || 0;
                                    return `${label}: ${value} ticket(s)`;
                                }
                            }
                        },
                    }
                }
            });
        },
        error: function () {
            console.error('Failed to load seat type ticket data');
        }
    });
}