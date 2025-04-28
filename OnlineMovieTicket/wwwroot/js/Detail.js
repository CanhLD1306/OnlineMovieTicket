const dateSelector = document.getElementById("dateSelector");
const weekdays = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];
const months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", 
                "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
const stripe = Stripe("pk_test_51QQyW6FAmwkm9Vtt5SE30P2PkBKuuw2yCea5855g7GnKvffPKPWN1rVRLHl4L5x4kHuUfOgwLrj2tZrWwEVcbNba004q0fitU0");
let room = null;
let cinema = null;
let startTime = null;
let showtimeId = null;
let dateSelected = null;
let selectedSeats = [];

$(document).ready(function () {
    $('#bookNowBtn').click(function(e) {
        if (!isLoggedIn) {
            const returnUrl = window.location.pathname + window.location.search;
            window.location.href = `/Identity/Account/Login?returnUrl=${encodeURIComponent(returnUrl)}`;
        }else {
            $('html, body').animate({
                scrollTop: $('#booking-area').offset().top
            }, 600);
        }

    });


    if (isLoggedIn && !isAdmin) {
        const elements = stripe.elements();
        const cardElement = elements.create("card");
        cardElement.mount("#card-element");
        loadCountries($('#countryFilter'));
        renderDateList();

        $('#countryFilter').on('change', function () {
            const countryId = $(this).val();
            $('#cityFilter').val("");
            $('#cinemaFilter').val("");
            selectedSeats= [];
            $('#bookingSeat').addClass('d-none');
            $('#seatSelected').empty();
            $('#bookingSummary').addClass('d-none');
            $('#showtimesSelector').empty();
            loadCities($('#cityFilter'), countryId);
            loadCinemas($('#cinemaFilter'));
            checkShowtimeVisibility();
        });

        $('#cityFilter').on('change', function () {
            const cityId = $(this).val();
            $('#cinemaFilter').val("");
            selectedSeats= [];
            $('#bookingSeat').addClass('d-none');
            $('#seatSelected').empty();
            $('#bookingSummary').addClass('d-none');
            $('#showtimesSelector').empty();
            loadCinemas($('#cinemaFilter'), cityId);
            checkShowtimeVisibility()
        });

        $('#cinemaFilter').on('change', function () {
            selectedSeats= [];
            $('#bookingSeat').addClass('d-none');
            $('#seatSelected').empty();
            $('#bookingSummary').addClass('d-none');
            $('#showtimesSelector').empty();
            checkShowtimeVisibility()
        });
    
        $(document).on("click", ".time-item", function () {
            $('.time-item').removeClass('active');
            $(this).addClass('active');
            showtimeId = $(this).data('showtime-id');
            cinema = $(this).data('cinema');
            room = $(this).data('room');
            startTime = $(this).data('starttime');
            $('#bookingSummary').addClass('d-none');
            loadSeatsForShowtime(showtimeId);
        });

        $(document).on("click", ".seat", function () {
            if ($(this).is('[disabled]')) {
                return;
            }
            const seatInfo = $(this).data('seat-info');
            const seatId = seatInfo.id;
            const index = selectedSeats.findIndex(s => s.id === seatId);
            if (index !== -1) {
            selectedSeats.splice(index, 1);
            $(this).css({
                'border': '1px solid #6c757d'
            });
            } else {
                selectedSeats.push(seatInfo);
                $(this).css({
                    'border': '5px solid red'
                });
            }
            updateBookingSummary();
            updateSelectedSeats();
            updateTotal();
            if (selectedSeats.length > 0) {
                $('#bookingSummary').removeClass('d-none');
            } else {
                $('#bookingSummary').addClass('d-none');
            }
        });
    
        $("#pay-button").click(async function (event) {
            event.preventDefault();
            
            const cardholderName = $("#cardName").val();
            const totalPrice = parseFloat($("#ticketTotalPrice").text());

            const { paymentMethod, error: paymentMethodError } = await stripe.createPaymentMethod({
                type: 'card',
                card: cardElement,
                billing_details: {
                    name: cardholderName
                }
            });

            if (paymentMethodError) {
                $("#card-errors").text(paymentMethodError.message);
                return;
            }

            $.ajax({
                url: urlCreatePaymentIntents,
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify({
                    amount: totalPrice
                }),
                success: async function (data) {
                    if (data.success) {
                        const clientSecret = data.clientSecret;
                        const { paymentIntent, error: confirmError } = await stripe.confirmCardPayment(clientSecret, {
                            payment_method: paymentMethod.id
                        });
                        if (confirmError) {
                            $("#card-errors").text(confirmError.message);
                        } else if (paymentIntent.status === 'succeeded') {
                            $.ajax({
                                url: urlCompletePayment,
                                type: "POST",
                                contentType: "application/json",
                                data: JSON.stringify({
                                    paymentIntentId: paymentIntent.id,
                                    ShowtimeSeatDTOs: selectedSeats,
                                    price: price
                                }),
                                success: function (result) {
                                    if (result.success) {
                                        toastr.success("Payment successful! Tickets have been created!");
                                        setTimeout(function () {
                                            window.location.reload();
                                        }, 2000);
                                    } else {
                                        toastr.error("An error occurred while creating tickets.");
                                    }
                                },
                                error: function (xhr) {
                                    toastr.error("Server error while completing the payment.");
                                }
                            });
                        }
                    } else {
                        toastr.error(data.error || "Unable to create payment intent.");
                    }
                },
                error: function (xhr) {
                    toastr.error("Server error while creating payment intent.");
                }
            });
        });
    }          
});
const dateItems = document.querySelectorAll('.date-item');
dateItems.forEach(item => {
    item.addEventListener('click', function() {
        dateItems.forEach(date => date.classList.remove('active'));
        this.classList.add('active');
        $('#bookingSeat').addClass('d-none');
        $('#seatSelected').empty();
        $('#bookingSummary').addClass('d-none');
    });
});
const cinemaItems = document.querySelectorAll('.cinema-item');
cinemaItems.forEach(item => {
    item.addEventListener('click', function() {
        cinemaItems.forEach(cinema => cinema.classList.remove('active'));
        this.classList.add('active');
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

function loadCountries(selectElement) {
    $.ajax({
        url: urlGetAllCountries,
        type: 'GET',
        success: function (data) {
            selectElement.val("");
            selectElement.find('option').not('[value=""]').remove();
            $.each(data, function (i, country) {
                selectElement.append($('<option>', {
                    value: country.id,
                    text: country.name,
                }));
            });
        },
        error: function () {
            toastr.error("Failed to load countries.");
        }
    });
}

function loadCities(selectElement, countryId = null) {
    $.ajax({
        url: urlGetCities,
        type: 'GET',
        data: {countryId: countryId},
        success: function (data) {
            selectElement.val("");
            selectElement.find('option').not('[value=""]').remove();
            if(!countryId){
                selectElement.prop('disabled', true);
            }else{
                selectElement.prop('disabled', false);
            }
            $.each(data, function (i, city) {
                selectElement.append($('<option>', {
                    value: city.id,
                    text: city.name,
                }));
            });
        },
        error: function () {
            toastr.error("Failed to load Cities.");
        }
    });
}

function loadCinemas(selectElement, cityId = null) {
    $.ajax({
        url: urlGetCinemas,
        type: 'GET',
        data: {cityId: cityId},
        success: function (data) {
            selectElement.val("");
            selectElement.find('option').not('[value=""]').remove();
            if(!cityId){
                selectElement.prop('disabled', true);
                return;
            }else{
                selectElement.prop('disabled', false);
            }
            $.each(data, function (i, cinema) {
                selectElement.append($('<option>', {
                    value: cinema.id,
                    text: cinema.name,
                }));
            });
        },
        error: function () {
            toastr.error("Failed to load Cinema.");
        }
    });
}

function renderDateList(startDate = new Date(), numberOfDays = 10) {
    const dateSelector = document.getElementById("dateSelector");
    dateSelector.innerHTML = "";

    const weekdays = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];
    const months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", 
                    "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

    for (let i = 0; i <= numberOfDays; i++) {
        const date = new Date(startDate);
        date.setDate(startDate.getDate() + i);

        const day = date.getDate();
        const month = months[date.getMonth()];
        const weekday = weekdays[date.getDay()];

        const dateItem = `
            <div class="date-item" data-date="${date.toISOString().split('T')[0]}">
                <span class="date-day">${day}</span>
                <span class="date-month">${month}</span>
                <span class="date-weekday">${weekday}</span>
            </div>
        `;
        dateSelector.innerHTML += dateItem;
    }
    document.querySelectorAll('.date-item').forEach(item => {
        item.addEventListener('click', function () {
            document.querySelectorAll('.date-item').forEach(d => d.classList.remove('active'));
            this.classList.add('active');
            dateSelected = this.getAttribute('data-date');
            selectedSeats= []
            $('#bookingSeat').addClass('d-none');
            $('#seatSelected').empty();
            $('#bookingSummary').addClass('d-none');
            checkShowtimeVisibility();
        });
    });
}

function checkShowtimeVisibility (){
    const cinemaSelected = $('#cinemaFilter').val();
    if(cinemaSelected && dateSelected){
        loadshowtimes()
        showtimesSelector.classList.remove('d-none');
    } else {
        showtimesSelector.classList.add('d-none');
    }
}


function loadshowtimes() {
    const cinemaId= $('#cinemaFilter').val();
    $.ajax({
        url: urlGetShowtimes,
        type: 'GET',
        data: {cinemaId: cinemaId, movieId: movieId, selectedDate: dateSelected},
        success: function (response) {
            $('#showtimesSelector').html(response);
        },
        error: function (xhr, status, error) {
            toastr.error('There was an error processing your request: ' + error);
        }
    });
}

function loadSeatsForShowtime(showtimeId){
    $.ajax({
        url: urlGetShowtimeSeats,
        type: 'GET',
        data: {showtimeId: showtimeId},
        success: function (response) {
            $("#bookingSeat").removeClass("d-none");
            $('#seatSelected').html(response);
        },
        error: function (xhr, status, error) {
            toastr.error('There was an error processing your request: ' + error);
        }
    });
}

function updateBookingSummary() {
    let cinemaSpan = document.getElementById("cinemaName");
    let roomSpan = document.getElementById("roomName");
    let startTimeSpan = document.getElementById("startTime");

    cinemaSpan.textContent = cinema;
    roomSpan.textContent = room;
    formattedTime = new Date(startTime).toLocaleString('en-US', {
                                weekday: 'long',
                                year: 'numeric',
                                month: 'long',
                                day: 'numeric',
                                hour: '2-digit',
                                minute: '2-digit',
                                hour12: false
    });
    startTimeSpan.textContent = formattedTime.replace(" at", " -");
}

function updateSelectedSeats() {
    let selectedSeatsSpan = document.getElementById("selectedSeats");

    const seatNames = selectedSeats.map(seat => {
        const rowLetter = String.fromCharCode('A'.charCodeAt(0) + seat.rowIndex);
        return rowLetter + seat.columnIndex;
    });

    selectedSeatsSpan.textContent = seatNames.join(', ');
}

function updateTotal() {
    let total = 0;
    let ticketTotalPriceSpan = document.getElementById("ticketTotalPrice");
    selectedSeats.forEach(seat => {
        total += (price * seat.priceMultiplier);
    });

    ticketTotalPriceSpan.textContent = total.toFixed(2);
}