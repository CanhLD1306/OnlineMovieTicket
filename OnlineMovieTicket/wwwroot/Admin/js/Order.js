let sortBy = "PurchaseDate";
let startDate = "";
let endDate = "";
let isDescending = true;
$(document).ready(function () {
    $('#ordersTable').DataTable({
        processing: true,
        serverSide: true,
        searching: false,
        ordering: false,
        lengthMenu: [5,10,25,50],
        ajax: {
            url: urlGetTickets,
            type: 'POST',
            data: function (d) {
                return {
                    Draw: d.draw,
                    SearchTerm: $('#searchTerm').val(),
                    StartDate: startDate,
                    EndDate: endDate,
                    PageNumber: (d.start / d.length) + 1,
                    PageSize: d.length,
                    SortBy: sortBy,
                    IsDescending: isDescending
                };
            }
        },  
        columns: [
            { "data": 'ticketCode'},
            { "data": 'user'},
            { "data": 'movie'},
            { "data": 'purchaseDate', 
                className: "text-center",
                "render": function (data, type, row) {
                    const date = new Date(data);
                    return date.toLocaleDateString('vi-VN');
                }
            },
            {"data": 'isPaid',
                className: 'text-center',
                render: function (data) {
                    const badgeClass = data === true ? 'badge-success' : 'badge-danger';
                    const text = data === true ? 'Success' : 'Fail';
                    return `<span class="badge ${badgeClass}">${text}</span>`;
                }
            },
        ],
        language: {
            emptyTable: "No data available in table",
            infoFiltered: "",
            paginate: {
                previous: '&lt;',
                next: '&gt;',
            }
        }
    });
    $('#searchTerm').on('keyup', function () {
        $('#ordersTable').DataTable().ajax.reload(null, false);
    });
    $('#startDate').on('change', function () {
        startDate = $(this).val();
        $('#endDate').attr('min', startDate);
        $('#ordersTable').DataTable().ajax.reload(null, false);
    });
    $('#endDate').on('change', function () {
        endDate = $(this).val();
        $('#startDate').attr('max', endDate);
        $('#ordersTable').DataTable().ajax.reload(null, false);
    });
});