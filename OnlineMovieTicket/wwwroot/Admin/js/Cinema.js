$(document).ready(function () {

    // List, search, filter and sort cinema in table

    let sortBy = "CreatedAt";
    let isDescending = true;
    let isAdjustingPage = false;
    const countrySelected = $('#countryFilter');
    const citySelected = $('#cityFilter');
    const statusSelected = $('#statusFilter');

    loadCountries(countrySelected, citySelected);

    $('#cinemasTable').DataTable({
        processing: true,
        serverSide: true,
        searching: false,
        ordering: false,
        lengthMenu: [5,10,25,50],
        ajax: {
            url: urlGetCinemas,
            type: 'POST',
            data: function (d) {
                return {
                    Draw: d.draw,
                    SearchTerm: $('#searchTerm').val(),
                    CountryId: countrySelected.val(),
                    CityId: citySelected.val(),
                    IsAvailable: statusSelected.val(),
                    PageNumber: (d.start / d.length) + 1,
                    PageSize: d.length,
                    SortBy: sortBy,
                    IsDescending: isDescending
                };
            }
        },  
        columns: [
            { "data": 'name' },
            { "data": 'countryName' },
            { "data": 'cityName' },
            { "data": 'totalRooms', className: "text-center"},
            { "data": 'isAvailable',
                className: "text-center",
                "render": function (data, type, row) {
                    const checked = data ? 'checked' : '';
                    return `
                        <div class="custom-control custom-switch">
                            <input type="checkbox" class="custom-control-input toggle-status" id="switch-${row.id}" data-id="${row.id}" ${checked}>
                            <label class="custom-control-label" for="switch-${row.id}"></label>
                        </div>`;    
                }
            },
            {
                "data": null,
                className: "text-center",
                "render": function (data, type, row) {
                    return `<a class='btn btn-sm btn-info btn-edit-cinema' title='Edit' data-id='${row.id}'>
                                <i class='fas fa-edit'></i>
                            </a>
                            <a class='btn btn-sm btn-danger ml-1 btn-delete-cinema' title='Delete' data-id='${row.id}'>
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

    $('#cinemasTable').on('draw.dt', function () {
        if (isAdjustingPage) {
            isAdjustingPage = false;
            return;
        }

        let table = $('#cinemasTable').DataTable();
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

    $('#searchTerm').on('input', function (e) {
        $('#cinemasTable').DataTable().ajax.reload();
    });

    $(document).on('click', '.sort-header', function (e) {
        const clickedSort = $(this).data('sort');
        toggleSort(clickedSort);
    });

    countrySelected.on('change', function () {
        const countryId = countrySelected.val();
        citySelected.val("")
        loadCities(citySelected, countryId);
        $('#cinemasTable').DataTable().ajax.reload();
    });

    citySelected.on('change', function () {
        $('#cinemasTable').DataTable().ajax.reload();
    });

    statusSelected.on('change', function () {
        $('#cinemasTable').DataTable().ajax.reload();
    });

    // Add new Cinema

    $('#addBtn').click(function(e) {
        e.preventDefault();
        $.ajax({
            url: urlAdd,
            type: 'GET',
            success: function (response) {
                $('#cinemaModal .modal-content').html(response);
                $.validator.unobtrusive.parse('#addCinemaForm');
                $('#cinemaModal').modal('show');
                const addCountrySelected = $('#addCountrySelected');
                const addCitySelected = $('#addCitySelected');
                loadCountries(addCountrySelected, addCitySelected);
            },
            error: function (xhr, status, error) {
                toastr.error('There was an error processing your request: ' + error);
            }
        });
    });

    $(document).on('change', '#addCountrySelected', function (e) {
        const countryId = $(this).val();
        const addCitySelected = $('#addCitySelected');
        addCitySelected.val("");
        addCitySelected.find('option').not('[value=""]').remove();
        if(!countryId){
            addCitySelected.prop('disabled', true);
            return;
        }
        addCitySelected.prop('disabled', false);
        loadCities(addCitySelected, countryId);
    });

    $(document).on('submit', '#addCinemaForm', function (e) {
        e.preventDefault();
        if (!$('#addCinemaForm').valid()) {
            return;
        }
        var token = $('input[name="__RequestVerificationToken"]').val();
        var formData = $(this).serialize();
        $.ajax({
            url: urlAdd,
            type: 'POST',
            data: formData,
            headers: {
                'RequestVerificationToken': token
            },
            success: function (response) {
                if (response.success) {
                    toastr.success(response.message);
                    $('#cinemaModal').modal('hide');
                    $('#cinemasTable').DataTable().ajax.reload(null, false);
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (xhr, status, error) {
                toastr.error('There was an error processing your request: ' + error);
            }
        });
    });

    // Edit cinema

    $(document).on('click', '.btn-edit-cinema', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        $.ajax({
            url: urlEdit,
            type: 'GET',
            data: { id: id },           
            success: function (response) {
                if (response) {
                    $('#cinemaModal .modal-content').html(response);
                    $.validator.unobtrusive.parse('#editCinemaForm');
                    $('#cinemaModal').modal('show');
                    const editCountrySelected = $('#editCountrySelected');
                    const editCitySelected = $('#editCitySelected');
                    const countryId = editCountrySelected.data('selected');
                    const cityId = editCitySelected.data('selected');
                    editCitySelected.prop('disabled', false);
                    loadCountries(editCountrySelected, editCitySelected, countryId, cityId);
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (xhr, status, error) {
                toastr.error('There was an error processing your request: ' + error);
            }
        });
    });

    $(document).on('change', '#editCountrySelected', function (e) {
        const countryId = $(this).val();
        const editCitySelected = $('#editCitySelected');
        editCitySelected.val("");
        editCitySelected.find('option').not('[value=""]').remove();
        if(!countryId){
            editCitySelected.prop('disabled', true);
            return;
        }
        editCitySelected.prop('disabled', false);
        loadCities(editCitySelected, countryId);
    });

    $(document).on('submit', '#editCinemaForm', function (e) {
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
                    $('#cinemaModal').modal('hide');
                    $('#cinemasTable').DataTable().ajax.reload(null, false);
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (xhr, status, error) {
                toastr.error('There was an error processing your request: ' + error);
            }
        });
    });

    // Delete cinema
    
    $(document).on('click', '.btn-delete-cinema', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        $.ajax({
            url: urlDelete,
            type: 'GET',
            data: { id: id },           
            success: function (response) {
                if (response) {
                    $('#cinemaModal .modal-content').html(response);
                    $('#cinemaModal').modal('show');
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (xhr, status, error) {
                toastr.error('There was an error processing your request: ' + error);
            }
        });
    });

    $(document).on('submit', '#deleteCinemaForm', function (e) {
        e.preventDefault();
        if (!$(this).valid()) {
            return;
        }
        var token = $('input[name="__RequestVerificationToken"]').val();
        var formData = $(this).serialize();
        $.ajax({
            url: urlDeleteConfirm,
            type: 'POST',
            data: formData,
            headers: {
                'RequestVerificationToken': token
            },
            success: function (response) {
                if (response.success) {
                    toastr.success(response.message);
                    $('#cinemaModal').modal('hide');
                    $('#cinemasTable').DataTable().ajax.reload(null, false);
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (xhr, status, error) {
                toastr.error('There was an error processing your request: ' + error);
            }
        });
    });

    // Change Status

    $(document).on('change', '.toggle-status', function () {
        const cinemaId = $(this).data('id');
        $.ajax({
            url: urlChangeStatus,
            type: 'POST',
            data: {id : cinemaId},
            success: function (response) {
                if (response.success) {
                    toastr.success(response.message);
                    $('#cinemasTable').DataTable().ajax.reload(null, false);
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (xhr, status, error) {
                toastr.error('There was an error processing your request: ' + error);
            }
        });
    });

    // Another, Funtion

    $(document).on('hidden.bs.modal', '.modal', function () {
        $('#cinemaModal .modal-content').html('');
    });

    function toggleSort(clickedSortBy) {
        if (sortBy === clickedSortBy) {
            isDescending = !isDescending;
        } else {
            sortBy = clickedSortBy;
            isDescending = true;
        }
        updateSortIcons();
        $('#cinemasTable').DataTable().ajax.reload();
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

    function loadCities(selectElement, countryId, CityId = null) {    
        $.ajax({
            url: urlGetAllCities,
            type: 'GET',
            data: { id: countryId },
            success: function (data) {
                selectElement.find('option').not('[value=""]').remove();
                $.each(data, function (i, city) {
                    selectElement.append($('<option>', {
                        value: city.id,
                        text: city.name,
                        selected: city.id == CityId
                    }));
                });
            },
            error: function (xhr, status, error) {
                toastr.error('Failed to load cities.');
            }
        });
    }

    function loadCountries(selectElement1, selectElement2, countryId = null, cityId = null){
        $.ajax({
            url: urlGetAllCountries,
            type: 'GET',
            success: function (data) {
                $.each(data, function (i, country) {
                    selectElement1.append($('<option>', {
                        value: country.id,
                        text: country.name,
                        selected: country.id == countryId
                    }));
                });
                loadCities(selectElement2, countryId, cityId);
            },
            error: function (xhr, status, error) {
                toastr.error('Failed to load countries.');
            }
        });
    }
});