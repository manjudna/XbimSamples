using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using XBIMApi.Model;

namespace XBIMApi.Controllers
{
    // [Route("api/[controller]")]
    [ApiController]
    public class ElementController : ControllerBase
    {
        private readonly ILogger<ElementController> _logger;

        public ElementController(ILogger<ElementController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("RoomArea")]
        public IActionResult Get()
        {
            List<RoomArea> roomAreas = null;
            try
            {
                roomAreas = GetRoomAreas();
                if (roomAreas != null)
                    return Ok(roomAreas);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            return NotFound(); 
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("ElementTypeCount")]
        public IActionResult GetElementTypes()
        {
            try
            {
                var elements = GetElementsCounts();
                if (elements != null)
                    return Ok(elements);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
            return NotFound();
        }

        private static IIfcValue GetArea(IIfcProduct product)
        {
            //try to get the value from quantities first
            var area =
                //get all relations which can define property and quantity sets
                product.IsDefinedBy

                //Search across all property and quantity sets. 
                //You might also want to search in a specific quantity set by name
                .SelectMany(r => r.RelatingPropertyDefinition.PropertySetDefinitions)

                //Only consider quantity sets in this case.
                .OfType<IIfcElementQuantity>()

                //Get all quantities from all quantity sets
                .SelectMany(qset => qset.Quantities)

                //We are only interested in areas 
                .OfType<IIfcQuantityArea>()

                //We will take the first one. There might obviously be more than one area properties
                //so you might want to check the name. But we will keep it simple for this example.
                .FirstOrDefault().AreaValue;

            if (area != null)
                return area;

            //try to get the value from properties
            return GetProperty(product, "Area");
        }
        private static IIfcValue GetProperty(IIfcProduct product, string name)
        {
            return
                //get all relations which can define property and quantity sets
                product.IsDefinedBy

                //Search across all property and quantity sets. You might also want to search in a specific property set
                .SelectMany(r => r.RelatingPropertyDefinition.PropertySetDefinitions)

                //Only consider property sets in this case.
                .OfType<IIfcPropertySet>()

                //Get all properties from all property sets
                .SelectMany(pset => pset.HasProperties)

                //lets only consider single value properties. There are also enumerated properties, 
                //table properties, reference properties, complex properties and other
                .OfType<IIfcPropertySingleValue>()

                //lets make the name comparison more fuzzy. This might not be the best practise
                .Where(p =>
                    string.Equals(p.Name, name, System.StringComparison.OrdinalIgnoreCase) ||
                    p.Name.ToString().ToLower().Contains(name.ToLower()))

                //only take the first. In reality you should handle this more carefully.
                .FirstOrDefault().NominalValue;
        }
        public static List<RoomArea> GetRoomAreas()
        {
            const string fileName = "SampleHouse.ifc";
           
            using (var model = IfcStore.Open(fileName))
            {

                var spaces = model.Instances.OfType<IIfcSpace>().ToList();

                List<RoomArea> roomArea = new List<RoomArea>();

                foreach (var space in spaces)
                {
                    var area = GetArea(space);
                    roomArea.Add(new RoomArea { Area = area.Value, Name = space.Name });
                }

                return roomArea;

            }
        }

        public static List<Element> GetElementsCounts()
        {
            const string fileName = "SampleHouse.ifc";

            using (var model = IfcStore.Open(fileName))
            {

                var ifcElements = model.Instances.OfType<IIfcElement>();

                var elementsDict = ifcElements
                 .GroupBy(item => item.ExpressType)
                 .ToDictionary(grp => grp.Key, grp => grp.ToList());
                List<Element> elements = new List<Element>();
                List<string> list = new List<string>();

                foreach (var item in elementsDict)
                {
                    var k = item.Key.Name;
                    var v = item.Value.Count;
                    elements.Add(new Element { Name = k, Count = v });
                }
                return elements;
             }
        }


      
    }
}
