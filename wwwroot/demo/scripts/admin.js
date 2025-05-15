$(document).ready(async function () {
    await signIn();

    $("#add-spaces").on("submit", function (e) {
        e.preventDefault();
        AddSpaces();
    });

    $("body").on("click", ".delete-space-button", function () {
        var spaceNumber = $(this).attr("data-space-number");
        deleteSpace(spaceNumber);
    });

    $("body").on("click", ".cancel-button", function () {
        var id = $(this).attr("data-id");
        cancelReservation(id);
    });

    $("body").on("click", ".confirm-button", function () {
        var id = $(this).attr("data-id");
        confirmReservation(id);
    });

    $("body").on("click", "#confirm-all-button", function () {
        confirmAll();
    });

    $("body").on("click", ".unblock-button", function () {
        var id = $(this).attr("data-id");
        unblockSpace(id);
    });

    $("body").on("click", ".detail-button", function () {
        var spaceNumber = $(this).attr("data-space-number");
        getSpaceDetail(spaceNumber);
    });

    $("body").on("click", ".block-dialog-button", function () {
        var spaceNumber = $(this).attr("data-space-number");
        blockSpaceDialog(spaceNumber);
    });

    $("body").on("submit", ".block-form", function (e) {
        e.preventDefault();
        var spaceNumber = $(this).attr("data-space-number");
        blockSpace(spaceNumber);
    });

    $("body").on("click", "#synchronize-button", function () {
        synchronizeUsers();
    });

    getSpaces();
    getRequests();
    getBlockings();
});

function AddSpaces() {
    $("#add-op-result").empty();
    $.ajax({
        type: "POST",
        url: 'https://localhost:7036/api/Spaces',
        data: JSON.stringify({
            count: $('#count').val()
        }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            displaySpaces(data);
            $('#add-spaces-result').append(`<p>Přidána místa (${data.lenght}).</p>`);
        },
        error: function (jqXHR) {
            alert(jqXHR.responseText)
        }
    });
}
function getSpaces() {
    $.ajax({
        url: 'https://localhost:7036/api/Spaces',
        method: 'GET',
        dataType: 'json',
        success: function (response) {
            displaySpaces(response)
        },
        error: function (jqXHR, textStatus) {
            console.error(textStatus, jqXHR);
        }
    });
}


function getRequests() {
    $.ajax({
        url: 'https://localhost:7036/api/Reservations/requests',
        method: 'GET',
        dataType: 'json',
        success: function (response) {
            for (var row of response) {
                $('#requests').append(`<tr id=res-${row.id}>
                <td>${row.id}</td>
                <td>${row.beginsAt}</td>
                <td>${row.endsAt}</td>
                <td>${row.spaceNumber}</td>
                <td>${row.createdAt}</td>
                <td>${row.displayName}</td>
                <td><button class="confirm-button" data-id=${row.id}>Potvrdit</button></td>
                <td><button class="cancel-button" data-id=${row.id}>Zrušit</button></td>
                </tr>`);
            }

        },
        error: function (jqXHR, textStatus) {
            console.error(textStatus, jqXHR);
        }
    });
}

function getBlockings() {
    $.ajax({
        url: 'https://localhost:7036/api/Blockings/my',
        method: 'GET',
        dataType: 'json',
        success: function (response) {
            for (var row of response) {
                displayBlocking(row);
            }

        },
        error: function (jqXHR, textStatus) {
            console.error(textStatus, jqXHR);
        }
    });
}

function getSpaceDetail(id) {
    $('.calendar').remove();
    $.ajax({
        url: `https://localhost:7036/api/Reservations/${id}`,
        method: 'GET',
        dataType: 'json',
        success: function (response) {
            $(`#sp-${id}`).after(
                `<tr class="calendar">
                    <td colspan=5>
                        <table class="calendar-table">
                            <tr>
                                <th>Číslo rezervace/blokace</th>
                                <th>Od</th>
                                <th>Do</th>
                                <th>Uživatel</th>
                                <th></th>
                            </tr>
                        </table>
                    <td>
                </tr>`
            )
            for (var row of response) {
                $('.calendar-table').append(`
                <tr id="cal-${row.id}">
                    <td>${row.id}</td>
                    <td>${row.beginsAt}</td>
                    <td>${row.endsAt}</td>
                    <td>${row.displayName}</td>

                </tr>`);
                if (row.$type == 'normal') {
                    $(`#cal-${row.id}`).append(
                        `<td><button class="cancel-button" data-id=${row.id}>Zrušit</button></td>`);
                }
                if (row.$type == 'blocking') {
                    $(`#cal-${row.id}`).append(
                        `<td>${row.comment}</td>`);
                }
            }

        },
        error: function (jqXHR) {
            alert(jqXHR.responseText)
        }
    });
}


function deleteSpace(id) {
    $.ajax({
        type: "DELETE",
        url: `https://localhost:7036/api/Spaces/${id}`,
        dataType: "json",
        success: function () {
            $(`#sp-${id}`).remove();
        },
        error: function (jqXHR) {
            console.log(jqXHR);
            alert(jqXHR.responseText)
        }
    });
}

function cancelReservation(id) {
    $.ajax({
        type: "DELETE",
        url: `https://localhost:7036/api/Reservations/${id}`,
        dataType: "json",
        success: function () {
            $(`#res-${id}`).remove();
            $(`#cal-${id}`).remove();
        },
        error: function (jqXHR) {
            alert(jqXHR.responseText)
        }
    });
}

function confirmReservation(id) {
    $.ajax({
        type: "PATCH",
        url: `https://localhost:7036/api/Reservations/${id}`,
        dataType: "json",
        success: function () {
            $(`#res-${id}`).remove();
        },
        error: function (jqXHR) {
            alert(jqXHR.responseText)
        }
    });
}

function confirmAll() {
    $.ajax({
        type: "PATCH",
        url: `https://localhost:7036/api/Reservations/confirm-all`,
        dataType: "json",
        success: function () {
            $("#requests").find("td").remove();
        },
        error: function (jqXHR, textStatus) {
            console.error(textStatus, jqXHR);
        }
    });
}

function unblockSpace(id) {
    $.ajax({
        type: "DELETE",
        url: `https://localhost:7036/api/Reservations/${id}`,
        dataType: "json",
        success: function () {
            $(`#bl-${id}`).remove();
        },
        error: function (jqXHR) {
            alert(jqXHR.responseText)
        }
    });
}

function blockSpace(spaceNumber) {
    $("#block-space-result").empty();
    $.ajax({
        type: "POST",
        url: 'https://localhost:7036/api/Blockings',
        data: JSON.stringify({
            beginsAt: $('#begins-at').val() + "Z",
            endsAt: $('#ends-at').val() + "Z",
            comment: $('#comment').val(),
            spaceNumber: spaceNumber,
        }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            $('.blocking-dialog').remove();
            $('#block-space-result').append(`<p>Vytvořena blokace číslo ${data.id}.</p>`);
            displayBlocking(data);
        },
        error: function (jqXHR) {
            alert(jqXHR.responseText)
        }
    });
}

function synchronizeUsers() {
    $("#synchronize-results").empty();
    $.ajax({
        type: "POST",
        url: `https://localhost:7036/api/Users/synchronize`,
        dataType: "json",
        success: function () {
            $("#synchronize-results").append("<span>Synchronizace proběhla úspěšně.</span>");
        },
        error: function (jqXHR, textStatus) {
            console.error(textStatus, jqXHR);
            $("#synchronize-results").append("<span>Synchronizace selhala.</span>");
        }
    });
}
function blockSpaceDialog(spaceNumber) {
    $('.blocking-dialog').remove();
    $(`#sp-${spaceNumber}`).after(
        `<tr class="blocking-dialog">
            <td>
                <form class="block-form" data-space-number=${spaceNumber}>
                    <label for="begins-at">Začátek:</label>
                    <input type="datetime-local" id="begins-at" name="begins-at" step"1800">
                    <label for="ends-at">Konec:</label>
                    <input type="datetime-local" id="ends-at" name="ends-at" step"1800">
                    <label for="comment">Komentář:</label>
                    <input type="text" id="comment" name="comment">
                    <input type="submit">
                </form>
            <td>
        </tr>`
    )
}

function displayBlocking(blocking) {
    $('#my-blockings').append(`<tr id="bl-${blocking.id}">
            <td>${blocking.id}</td>
            <td>${blocking.beginsAt}</td>
            <td>${blocking.endsAt}</td>
            <td>${blocking.spaceNumber}</td>
            <td>${blocking.createdAt}</td>
            <td>${blocking.comment}</td>
            <td><button class="unblock-button" data-id=${blocking.id}>Zrušit</button></td>
        </tr>`);
}

function displaySpaces(spaces) {
    for (var space of spaces) {
        $('#all-spaces').append(`<tr id="sp-${space.spaceNumber}">
                <td>${space.spaceNumber}</td>
                <td>${space.createdAt}</td>
                <td><button class="detail-button" data-space-number=${space.spaceNumber}>Detail</button></td>
                <td><button class="block-dialog-button" data-space-number=${space.spaceNumber}>Blokovat</button</td>
                <td><button class="delete-space-button" data-space-number=${space.spaceNumber}>Odstranit</button></td>
            </tr>`);
    }
}