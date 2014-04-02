
(function ($) {
    $.fn.loadUrl = function (url, postData, callback) {
        var self = this;

        if (typeof(postData) == "function") {
            callback = postData;
            postData = null;
        }

        self.fadeOut(function () {
            self
                .html("<span class='loading'></span>")
                .show();
        });

        $.ajax({
            type: postData != null ? "POST" : "GET",
            url: url,
            data: postData,
            success: function (responseText) {
                self
                    .stop(true, true)
                    .show()
                    .html(responseText)
                ;

                if (callback != null) {
                    callback.call(self, true);
                }
            },
            error: function (req, textStatus, errorThrown) {
                self
                    .stop(true, true)
                    .show()
                    .html(req.responseText)
                ;

                if (callback != null) {
                    callback.call(self, false);
                }
            }
        });
    };

    $.fn.initializeEditors = function () {
        this
            .find(".mceEditor")
            .each(function () {
                // This should not be necessary, but in case some code forgot to
                // destroy an editor with that id, we'll destroy it here.
                if (tinyMCE.getInstanceById(this.id)) {
                    tinyMCE.execCommand('mceRemoveControl', false, this.id);
                }
                tinyMCE.execCommand("mceAddControl", false, this.id);
            });

        return this;
    };

    $.fn.disposeEditors = function () {
        this
            .find(".mceEditor")
            .each(function () {
                //tinyMCE.execCommand('mceFocus', false, this.id);
                tinyMCE.execCommand('mceRemoveControl', false, this.id);
            });

        return this;
    };
})(jQuery);
