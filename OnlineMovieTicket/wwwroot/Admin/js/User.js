$(document).ready(function () {
    $('#searchTerm').on('input', function (e) {
        $('#usersTable').DataTable().ajax.reload();
    });

    $('#genderFilter').on('change', function () {
        $('#usersTable').DataTable().ajax.reload();
    });

    $('#lockedFilter').on('change', function () {
        $('#usersTable').DataTable().ajax.reload();
    });

    $('#usersTable').DataTable({
        processing: true,
        serverSide: true,
        searching: false,
        ordering: false,
        lengthMenu: [5,10,25,50],
        ajax: {
            url: urlGetUsers,
            type: 'POST',
            data: function (d) {
                return {
                    Draw: d.draw,
                    SearchTerm: $('#searchTerm').val(),
                    Gender: $('#genderFilter').val(),
                    IsLocked: $('#lockedFilter').val(),
                    PageNumber: (d.start / d.length) + 1,
                    PageSize: d.length,
                    PageNumber: 1
                };
            }
        },  
        columns: [
            { "data": 'email' },
            {
                "data": null,
                "render": function (data, type, row) {
                    return `${row.firstName ?? ''} ${row.lastName ?? ''}`;
                }
            },
            { "data": 'phoneNumber', className: 'text-center'},
            { "data": 'genderName', className: 'text-center'},
            {"data": 'isLockedOut',
                className: 'text-center',
                render: function (data) {
                    const badgeClass = data === true ? 'badge-danger' : 'badge-success';
                    const text = data === true ? 'Locked' : 'Active';
                    return `<span class="badge ${badgeClass}">${text}</span>`;
                }
            },
            {
                "data": null,
                className: 'text-center',
                "render": function (data, type, row) {
                    const lockButtonClass = row.isLockedOut ? 'btn-success' : 'btn-danger';
                    const lockButtonText = row.isLockedOut ? 'Unlock' : 'Lock';
                    return `<button class="btn ${lockButtonClass} btn-sm lock-unlock-btn" data-user-email="${row.email}">${lockButtonText}</button>`;
                }
            }
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

    $(document).on('click', '.lock-unlock-btn', function (e) {
        e.preventDefault();
        var email = $(this).data('user-email');
        $.ajax({
            url: urlGetUser,
            type: 'GET',
            data: { email: email },           
            success: function (response) {
                if (response) {
                    $('#userModal .modal-content').html(response);
                    $.validator.unobtrusive.parse('#LockOrUnlockForm');
                    $('#userModal').modal('show');
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (xhr, status, error) {
                toastr.error('There was an error processing your request: ' + error);
            }
        });
    });

    $(document).on('submit', '#lockOrUnlockForm', function (e) {
        e.preventDefault();
        if (!$(this).valid()) {
            return;
        }
        var token = $('input[name="__RequestVerificationToken"]').val();
        var formData = $(this).serialize();
        $.ajax({
            url: urlLockOrUnlockUser,
            type: 'POST',
            data: formData,
            headers: {
                'RequestVerificationToken': token
            },
            success: function (response) {
                if (response.success) {
                    toastr.success(response.message);
                    $('#usersTable').DataTable().ajax.reload(null, false);
                    $('#userModal').modal('hide');
                } else {
                    toastr.error(response.message);
                }
            },
            error: function () {
                toastr.error('An error occurred while processing the request.');
            }
        });
    });
});