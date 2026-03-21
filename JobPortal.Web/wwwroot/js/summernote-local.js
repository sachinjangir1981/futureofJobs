$(document).ready(function () {
    $('.summernote').summernote({
        height: 200,
        fontSizes: ['8', '9', '10', '11', '12', '14', '16', '18', '20', '22', '24', '36'],
        callbacks: {
            onPaste: function (e) {
                var bufferText = ((e.originalEvent || e).clipboardData || window.clipboardData).getData('Text');
                e.preventDefault();
                //  var newtext = getCleanHtml(bufferText);
                // Firefox can produce different tags when pasting from Word, so check for them
                // Also, prevent Summernote's default paste behavior and insert clean text.
                setTimeout(function () {
                    document.execCommand('insertText', false, cleanHtmlFromWord(bufferText));
                }, 2000);
            }

        },
        toolbar: [
            ['style', ['bold', 'italic', 'underline', 'clear']],
            ['font', ['strikethrough', 'superscript', 'subscript']],
            ['fontsize', ['fontsize']],
            ['color', ['color']],
            ['para', ['ul', 'ol', 'paragraph']],
            ['height', ['height']],
            ['table', ['table']],
            ['insert', ['link', 'picture', 'video']],
            ['view', ['fullscreen', 'codeview', 'help']]
        ]
    });
});



function cleanHtmlFromWord(input) {
    // 1. Remove line breaks / Mso classes
    var output = input.replace(/(class=(")?Mso[a-zA-Z]+(")?)/g, ' ');

    // 2. Strip Word generated HTML comments
    //  output = output.replace(///g, '');

    // 3. Remove common Word junk tags (meta, link, span, xml, st1, o, font)
    var tagStripper = new RegExp('<(/)*(meta|link|span|\\?xml:|st1:|o:|font)(.*?)>', 'gi');
    output = output.replace(tagStripper, '');

    // // 4. Remove everything in between and including tags like style, script, applet, embed, noframes, noscript
    // var badTags = ['style', 'script', 'applet', 'embed', 'noframes', 'noscript'];
    // for (var i = 0; i < badTags.length; i++) {
    //     var regex = new RegExp('<' + badTags[i] + '.*?' + badTags[i] + '(.*?)>', 'gi');
    //     output = output.replace(regex, '');
    // }

    // // 5. Remove attributes like 'style="..."' and 'start="..."'
    // var badAttributes = ['style', 'start'];
    // for (var i = 0; i < badAttributes.length; i++) {
    //     var attributeStripper = new RegExp(' ' + badAttributes[i] + '="(.*?)"', 'gi');
    //     output = output.replace(attributeStripper, '');
    // }

    // // Optionally: Convert strong/em to b/i, etc., if you want simpler tags
    // output = output.replace(/<strong[^>]*>/gi, '<b>');
    // output = output.replace(/<\/strong>/gi, '</b>');
    // output = output.replace(/<em[^>]*>/gi, '<i>');
    // output = output.replace(/<\/em>/gi, '</i>');

    // // Remove empty paragraphs
    // output = output.replace(/<p>\s*&nbsp;\s*<\/p>/gi, '');
    // output = output.replace(/<p>\s*<\/p>/gi, '');

    return output;
}