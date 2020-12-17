function search_flights() {
    $("#flightsData").dataTable({
        "searching": false,
        "processing": "true",
        "order": [],
        "columnDefs": [{
            orderable: false,
            targets: "no-sort"
        }],
        "lengthChange": false,
        "ajax": {
            "url": "/Home/GetFlights",
            "type": "POST",
            "datatype": "json",
            "data": function (d) {
                return $.extend({}, d, {
                    "OriginAirport": $('#OriginAirport').val(),
                    "DestinationAirport": $('#DestinationAirport').val(),
                    "DepartureDate": $('#DepartureDate').val(),
                    "ReturnDate": $('#ReturnDate').val(),
                    "NumberOfPassengers": $('#NumberOfPassengers').val(),
                    "Currency": $('#Currency').val()
                });
            },
            "dataSrc": function (json) {
                var jsonData = JSON.stringify(json.data)
                sessionStorage.setItem($('#OriginAirport').val() + $('#DestinationAirport').val() + $('#DepartureDate').val() + $('#ReturnDate').val() + $('#NumberOfPassengers').val() + $('#Currency').val(), jsonData);
                return json.data  
            },
            "error": function () {
                alert("Wrong Client ID and Api Key");
            }
        },

        "columns": [
            { "data": "departureAirport"},
            { "data": "arrivalAirport"},
            { "data": "departureDate"},
            { "data": "returnDate"},
            { "data": "departureConnections"},
            { "data": "returnConnections"},
            { "data": "numberOfPassengers"},
            { "data": "currency"},
            { "data": "price" }
        ]
    });
}

function retrieve_data() {  
    $("#flightsData").DataTable().clear();
    $("#flightsData").DataTable().rows.add(JSON.parse(sessionStorage.getItem($('#OriginAirport').val() + $('#DestinationAirport').val() + $('#DepartureDate').val() + $('#ReturnDate').val() + $('#NumberOfPassengers').val() + $('#Currency').val()))).draw();
}

$(document).ready(function () {

    $("#flightsData").hide();

    $("#flightsForm").validate({
        rules: {
            originAirport: "required",
            destinationAirport: "required",
            departureDate: "required"
        },
        messages: {
            originAirport: "Origin airport required!",
            destinationAirport: "Destination airport required!",
            departureDate: "Departure date required!"
        }
    });

    $("#search_button").on("click", function () {

        if ($("#flightsForm").valid()) {

            // Check if dataTable is created or not
            if (!$.fn.DataTable.isDataTable("#flightsData")) {
                search_flights(); // Create dataTable
                $("#flightsData").show();
            }
            else {
                // Check if search parameters exist
                if (sessionStorage.getItem($('#OriginAirport').val() + $('#DestinationAirport').val() + $('#DepartureDate').val() + $('#ReturnDate').val() + $('#NumberOfPassengers').val() + $('#Currency').val())) {
                    retrieve_data();
                }
                else {
                    $("#flightsData").DataTable().ajax.reload();
                }
            }
        }
    })
});

