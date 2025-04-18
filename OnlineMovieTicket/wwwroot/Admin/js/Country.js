$(document).ready(function () {

    // List, search, filter and sort country in table

    let sortBy = "CreatedAt";
    let isDescending = true;
    let isAdjustingPage = false;

    $('#countriesTable').DataTable({
        processing: true,
        serverSide: true,
        searching: false,
        ordering: false,
        lengthMenu: [5,10,25,50],
        ajax: {
            url: urlGetCountries,
            type: 'POST',
            data: function (d) {
                return {
                    Draw: d.draw,
                    SearchTerm: $('#searchTerm').val(),
                    PageNumber: (d.start / d.length) + 1,
                    PageSize: d.length,
                    SortBy: sortBy,
                    IsDescending: isDescending
                };
            }
        },  
        columns: [
            { "data": 'name' },
            { "data": 'code' },
            {
                "data": null,
                className: "text-center",
                "render": function (data, type, row) {
                    return `<a class='btn btn-sm btn-info btn-edit-country' title='Edit' data-id='${row.id}'>
                                <i class='fas fa-edit'></i>
                            </a>
                            <a class='btn btn-sm btn-danger ml-1 btn-delete-country' title='Delete' data-id='${row.id}'>
                                <i class='fas fa-trash'></i>
                            </a>`;
                }
            }
        ],
        language: {
            emptyTable: "No data available in table",
            infoFiltered: "",
        }
    });

    $('#searchTerm').on('input', function (e) {
        $('#countriesTable').DataTable().ajax.reload();
    });

    $(document).on('click', '.sort-header', function (e) {
        e.preventDefault();
        const clickedSort = $(this).data('sort');
        toggleSort(clickedSort);
    });

    // Add new Country

    $('#addBtn').click(function(e) {
        e.preventDefault();
        $.ajax({
            url: urlCreate,
            type: 'GET',
            success: function (response) {
                $('#countryModal .modal-content').html(response);
                $.validator.unobtrusive.parse('#addCountryForm');
                $('#countryModal').modal('show');
            },
            error: function (xhr, status, error) {
                toastr.error('There was an error processing your request: ' + error);
            }
        });
    });

    $(document).on('submit', '#addCountryForm', function (e) {
        e.preventDefault();
        if (!$('#addCountryForm').valid()) {
            return;
        }
        var token = $('input[name="__RequestVerificationToken"]').val();
        var formData = $(this).serialize();
        $.ajax({
            url: urlCreate,
            type: 'POST',
            data: formData,
            headers: {
                'RequestVerificationToken': token
            },
            success: function (response) {
                if (response.success) {
                    toastr.success(response.message);
                    $('#countryModal').modal('hide');
                    $('#countriesTable').DataTable().ajax.reload(null, false);
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (xhr, status, error) {
                toastr.error('There was an error processing your request: ' + error);
            }
        });
    });

    // Edit country

    $(document).on('click', '.btn-edit-country', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        $.ajax({
            url: urlEdit,
            type: 'GET',
            data: { countryId: id },           
            success: function (response) {
                if (response) {
                    $('#countryModal .modal-content').html(response);
                    $.validator.unobtrusive.parse('#editCountryForm');
                    $('#countryModal').modal('show');
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (xhr, status, error) {
                toastr.error('There was an error processing your request: ' + error);
            }
        });
    });

    $(document).on('submit', '#editCountryForm', function (e) {
        e.preventDefault();
        if (!$(this).valid()) {
            return;
        }
        var token = $('input[name="__RequestVerificationToken"]').val();
        var formData = $(this).serialize();
        $.ajax({
            url: urlEdit,
            type: 'POST',
            data: formData,
            headers: {
                'RequestVerificationToken': token
            },
            success: function (response) {
                if (response.success) {
                    toastr.success(response.message);
                    $('#countryModal').modal('hide');
                    $('#countriesTable').DataTable().ajax.reload(null, false);
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (xhr, status, error) {
                toastr.error('There was an error processing your request: ' + error);
            }
        });
    });

    // Delete country
    $(document).on('click', '.btn-delete-country', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        $.ajax({
            url: urlDelete,
            type: 'GET',
            data: { countryId: id },           
            success: function (response) {
                if (response) {
                    $('#countryModal .modal-content').html(response);
                    $('#countryModal').modal('show');
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (xhr, status, error) {
                toastr.error('There was an error processing your request: ' + error);
            }
        });
    });

    $(document).on('submit', '#deleteCountryForm', function (e) {
        e.preventDefault();
        if (!$(this).valid()) {
            return;
        }
        var token = $('input[name="__RequestVerificationToken"]').val();
        var formData = $(this).serialize();
        $.ajax({
            url: urlDelete,
            type: 'POST',
            data: formData,
            headers: {
                'RequestVerificationToken': token
            },
            success: function (response) {
                if (response.success) {
                    toastr.success(response.message);
                    $('#countryModal').modal('hide');
                    $('#countriesTable').DataTable().ajax.reload(null, false);
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (xhr, status, error) {
                toastr.error('There was an error processing your request: ' + error);
            }
        });
    });

    // Another, Function

    $(document).on('hidden.bs.modal', '.modal', function () {
        $('#countryModal .modal-content').html('');
    });

    $('#countriesTable').on('draw.dt', function () {
        if (isAdjustingPage) {
            isAdjustingPage = false;
            return;
        }

        let table = $('#countriesTable').DataTable();
        let pageInfo = table.page.info();
        let currentPage = pageInfo.page;
        let currentPageRowCount = table.rows({ page: 'current' }).count();

        if (currentPageRowCount === 0 && currentPage > 0) {
            isAdjustingPage = true;
            setTimeout(() => {
                table.page(currentPage - 1).draw(false);
            }, 0);
        }
    });
});

// Functions

function toggleSort(clickedSortBy) {
    if (sortBy === clickedSortBy) {
        isDescending = !isDescending;
    } else {
        sortBy = clickedSortBy;
        isDescending = true;
    }
    updateSortIcons();
    $('#countriesTable').DataTable().ajax.reload();
}

function updateSortIcons() {
    $('.sort-header i').removeClass('fa-sort-up fa-sort-down').addClass('fa-sort');
    const iconId = `#sort-icon-${sortBy}`;
    $(iconId).removeClass('fa-sort');
    if (isDescending) {
        $(iconId).addClass('fa-sort-down');
    } else {
        $(iconId).addClass('fa-sort-up');
    }
}