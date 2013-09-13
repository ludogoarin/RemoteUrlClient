var clientParser = {
    init: function () {
        $("#remoteSource").html("No HTML retrieved yet.");
    },
    loadUrl: function (url) {
        var $resBlock = $("#remoteSource");
        var $preview = $('#ifPreview');
        var $alerts = $("#alertContainer");

        // prepare
        $alerts.html('');
        $("#btGetUrl").text("loading...");
        $preview.contents().find('html').html('');
        $resBlock.html('');

        $.post('/root/preview', { url: url }, function (data) {
            $resBlock.text(data.Source);
            try {
                $preview.contents().find('html').html(data.UpdatedSource);
            } catch (e) {
                // do nothing
            }
        })
		.done(function () {
		    $alerts.prepend('<div class="alert alert-success alert-dismissable">' +
				'<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
				'Successfully retrieved HTML from URL:<br>' + url + '</div>');
		})
		.fail(function (jqXHR, textStatus, errorThrown) {
		    $alerts.prepend('<div class="alert alert-warning alert-dismissable">' +
				'<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
				'Error occured retrieving ' + url + ' (' + textStatus + ')</div>');
		    $resBlock.text(textStatus);
		    console.log(errorThrown);
		    console.log(jqXHR);
		})
		.always(function () {
		    $("#btGetUrl").text("Submit");
		});
    },
}

// page level code
$(function () {
    // prevent form from being submitted
    $("form").on("submit", function () {
        return false;
    });

    clientParser.init();

    $("#btGetUrl").on("click", function () {
        var url = $("#tiRemoteUrl").val();
        console.log(url.length);
        console.log(url.length > 0);
        console.log(url.indexOf('http'));
        console.log(url.indexOf('http') > -1);
        if (url.length > 0 && url.indexOf('http') > -1) {
            clientParser.loadUrl(url);
        } else {
            alert("Please enter a valid URL");
        }
    });

    $("#tiRemoteUrl").on("focus", function () {
        $(this).val('');
    });

})
