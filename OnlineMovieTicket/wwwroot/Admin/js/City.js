$(document).ready(function () {

    // List, search, filter and sort city in table

    let sortBy = "CreatedAt";
    let isDescending = true;
    let isAdjustingPage = false;

    loadCountries($('#countryFilter'));

    $('#citiesTable').DataTable({
        processing: true,
        serverSide: true,
        searching: false,
        ordering: false,
        lengthMenu: [5,10,25,50],
        ajax: {
            url: urlGetCities,
            type: 'POST',
            data: function (d) {
                return {
                    Draw: d.draw,
                    SearchTerm: $('#searchTerm').val(),
                    CountryId: $('#countryFilter').val(),
                    PageNumber: (d.start / d.length) + 1,
                    PageSize: d.length,
                    SortBy: sortBy,
                    IsDescending: isDescending
                };
            }
        },  
        columns: [
            { "data": 'name' },
            { "data": 'postalCode' },
            { "data": 'countryName' },
            {
                "data": null,
                className: "text-center",
                "render": function (data, type, row) {
                    return `<a class='btn btn-sm btn-info btn-edit-city' title='Edit' data-id='${row.id}'>
                                <i class='fas fa-edit'></i>
                            </a>
                            <a class='btn btn-sm btn-danger ml-1 btn-delete-city' title='Delete' data-id='${row.id}'>
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
        $('#citiesTable').DataTable().ajax.reload();
    });

    $(document).on('click', '.sort-header', function (e) {
        e.preventDefault();
        const clickedSort = $(this).data('sort');
        toggleSort(clickedSort);
    });

    $('#countryFilter').on('change', function () {
        const selectedCountryId = $(this).val();
        $('#citiesTable').DataTable().ajax.reload();
    });

    // Add new City

    $('#addBtn').click(function(e) {
        e.preventDefault();
        $.ajax({
            url: urlCreate,
            type: 'GET',
            success: function (response) {
                $('#cityModal .modal-content').html(response);
                $.validator.unobtrusive.parse('#addCityForm');
                $('#cityModal').modal('show');
                setTimeout(() => {
                    const addCountrySelected = $('#addCountrySelected');
                    const countryId = addCountrySelected.val();
                    loadCountries(addCountrySelected);
                }, 100);
            },
            error: function (xhr, status, error) {
                toastr.error('There was an error processing your request: ' + error);
            }
        });
    });

    $(document).on('submit', '#addCityForm', function (e) {
        e.preventDefault();
        if (!$('#addCityForm').valid()) {
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
                    $('#cityModal').modal('hide');
                    $('#citiesTable').DataTable().ajax.reload(null, false);
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (xhr, status, error) {
                toastr.error('There was an error processing your request: ' + error);
            }
        });
    });

    // Edit city

    $(document).on('click', '.btn-edit-city', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        $.ajax({
            url: urlEdit,
            type: 'GET',
            data: { cityId: id },           
            success: function (response) {
                if (response) {
                    $('#cityModal .modal-content').html(response);
                    $.validator.unobtrusive.parse('#editCityForm');
                    $('#cityModal').modal('show');
                    const editCountrySelected = $('#editCountrySelected');
                    const countryId = editCountrySelected.data('selected');
                    loadCountries(editCountrySelected, countryId);
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (xhr, status, error) {
                toastr.error('There was an error processing your request: ' + error);
            }
        });
    });

    $(document).on('submit', '#editCityForm', function (e) {
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
                    $('#cityModal').modal('hide');
                    $('#citiesTable').DataTable().ajax.reload(null, false);
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (xhr, status, error) {
                toastr.error('There was an error processing your request: ' + error);
            }
        });
    });

    // Delete city
    $(document).on('click', '.btn-delete-city', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        $.ajax({
            url: urlDelete,
            type: 'GET',
            data: { cityId: id },           
            success: function (response) {
                if (response) {
                    $('#cityModal .modal-content').html(response);
                    $('#cityModal').modal('show');
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (xhr, status, error) {
                toastr.error('There was an error processing your request: ' + error);
            }
        });
    });

    $(document).on('submit', '#deleteCityForm', function (e) {
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
                    $('#cityModal').modal('hide');
                    $('#citiesTable').DataTable().ajax.reload(null, false);
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (xhr, status, error) {
                toastr.error('There was an error processing your request: ' + error);
            }
        });
    });

    // Another

    $(document).on('hidden.bs.modal', '.modal', function () {
        $('#cityModal .modal-content').html('');
    });

    $('#citiesTable').on('draw.dt', function () {
        if (isAdjustingPage) {
            isAdjustingPage = false;
            return;
        }

        let table = $('#citiesTable').DataTable();
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
    $('#citiesTable').DataTable().ajax.reload();
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

function loadCountries(selectElement, countryId = null){
    $.ajax({
        url: urlGetAllCountries,
        type: 'GET',
        success: function (data) {
            $.each(data, function (i, country) {
                selectElement.append($('<option>', {
                    value: country.id,
                    text: country.name,
                    selected: country.id == countryId
                }));
            });
        },
        error: function (xhr, status, error) {
            toastr.error('Failed to load countries.');
        }
    });
}