$(document).ready(async function () {

    await signIn();

    $("#get-spaces").on("submit", function (e) {
        e.preventDefault();
        getSpaces();
    });

    $("body").on("click", "#res-button", makeReservation);

    $("body").on("click", ".cancel-button", function () {
        var id = $(this).parent().attr("id");
        cancelReservation(id);
    });

    getMyReservations();
});

function getSpaces() {
    $.ajax({
        url: `https://localhost:7036/api/ParkingSpaces/avialible?from=${$('#begins-at').val()}Z&till=${$('#ends-at').val()}Z`,
        method: 'GET',
        dataType: 'json',
        success: function (response) {
            $('#avialible').empty();
            $('#avialible').append(`<span>Obsazeno ${response.occupiedCount}/${response.totalCount}</span>`);
            if (response.avialible) {
                $('#avialible').append(`<button id="res-button">Rezervovat</span>`);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error('Error:', textStatus, errorThrown);
        }
    });
}

function makeReservation() {
    $("#make-reservation-result").empty();
    $.ajax({
        type: "POST",
        url: 'https://localhost:7036/api/Reservations',
        data: JSON.stringify({
            beginsAt: $('#begins-at').val() + "Z",
            endsAt: $('#ends-at').val() + "Z",
        }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            displayReservation(data);
            $('#make-reservation-result').append(`<p>Vytvořen požadavek na rezervaci číslo ${data.id}.</p>`);
        },
        error: function (data) {
            $('#make-reservation-result').append(`<p>${data.responseText}</p>`);
        }
    });
}

function getMyReservations() {
    $.ajax({
        url: 'https://localhost:7036/api/Reservations/my',
        method: 'GET',
        dataType: 'json',
        success: function (response) {
            for (var row of response) {
                displayReservation(row);
            }

        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error('Error:', textStatus, errorThrown);
        }
    });
}

function cancelReservation(id) {
    $.ajax({
        type: "DELETE",
        url: `https://localhost:7036/api/Reservations/${id}`,
        success: function () {
            $(`#${id}`).find(".state").text(3);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error('Error:', textStatus, errorThrown);
        }
    });
}

function displayReservation(reservation) {
    $('#my').append(`<tr id=${reservation.id}>
            <td>${reservation.id}</td>
            <td>${reservation.beginsAt}</td>
            <td>${reservation.endsAt}</td>
            <td>${reservation.spaceNumber}</td>
            <td>${reservation.issuedAt}</td>
            <td class="state">${reservation.stateId}</td>
            <td><button class="cancel-button">Zrušit</button></td>
        </tr>`);
}