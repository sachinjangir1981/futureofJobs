  var exhibitorsData = [];
$(function () {

   

    $('.list-group-item').each(function (index) {

        const $item = $(this); // cache selector (important for performance)

        let imageSrc = $item.find('.img_event_box img').attr('src') || '';
        let website = $item.find('.button_block a').attr('href') || '';
        let name= $item.find('.heading').text().trim();
        let categories= $item.find('.sector_block_outer').text().trim();
      

        exhibitorsData.push({
            index: index + 1,
            image: imageSrc,
            website: website,
            titles: name,
            category:categories

        });
    });

    console.log(exhibitorsData);

    
});
 
function senddata()
{
      console.log(exhibitorsData);

      $('#hidid').val(JSON.stringify(exhibitorsData));
    $('#frmid').submit();
}