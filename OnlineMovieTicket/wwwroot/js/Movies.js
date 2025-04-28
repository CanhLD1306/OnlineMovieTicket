$(document).ready(function () {
    loadMovies(1, true);

    $('#movieSearch').on('input', function () {
        loadMovies(1, true);
    });

    $('#movieStatusFilter, #startDateFilter, #endDateFilter').on('change', function () {
        loadMovies(1, true);
    });

    $('#resetFilters').on('click', function () {
        $('#movieSearch').val('');
        $('#movieStatusFilter').val('');
        $('#startDateFilter').val('');
        $('#endDateFilter').val('');
        loadMovies(1, true);
    });
});
function showTrailer(youtubeUrl) {
    const embedUrl = youtubeUrl.replace("watch?v=", "embed/");
    $('#trailerIframe').attr('src', embedUrl + '?autoplay=1');
    
    var trailerModal = new bootstrap.Modal(document.getElementById('trailerModal'));
    trailerModal.show();
}

function stopTrailer() {
    $('#trailerIframe').attr('src', '');
}

function loadMovies(pageNumber, setupPaging = false) {
    startDate = $('#startDateFilter').val() || null;
    endDate = $('#endDateFilter').val() || null;
    $.ajax({
        url: urlGetMovies,
        type: 'GET',
        data: {
            SearchTerm: $('#movieSearch').val(),
            StartDate: startDate,
            EndDate: endDate,
            IsCommingSoon: $('#movieStatusFilter').val(),
            PageNumber: pageNumber,
            PageSize: 12,
            SortBy: 'ReleaseDate',
            IsDescending: true
        },
        success: function (response) {
            $('#movie-grid').html(response);
            const filterCount = parseInt($('#filterCount').val(), 10);
            if (setupPaging) {
                setupPagination(filterCount, pageNumber);
            }
        },
        error: function () {
            console.error('Error loading movies.');
        }
    });
}
function setupPagination(totalItems, currentPage = 1) {
    const totalPages = Math.ceil(totalItems / 12);
    $('#pagination').twbsPagination('destroy');
    if (totalPages > 1) {
        $('#pagination').twbsPagination({
            totalPages: totalPages,
            visiblePages: 5,
            startPage: currentPage,
            initiateStartPageClick: false,
            prev: '&lt;',
            next: '&gt;',
            first: null,
            last: null,
            onPageClick: function (event, page) {
                loadMovies(page, false);
                $('html, body').animate({ scrollTop: 0 }, 'fast');
            }
        });
    } else {
        $('#pagination').html('');
    }
}