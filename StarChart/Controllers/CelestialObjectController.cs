using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using StarChart.Models;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.FirstOrDefault(co => co.Id == id);

            if (celestialObject == null)
            {
                return NotFound();
            }

            celestialObject.Satellites = _context.CelestialObjects.Where(co => co.OrbitedObjectId == celestialObject.Id).ToList();

            return Ok(celestialObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObject = _context.CelestialObjects.Where(co => string.Equals(name, co.Name)).ToList();

            if (celestialObject.Count < 1)
            {
                return NotFound();
            }

            foreach (CelestialObject co in celestialObject)
            {
                co.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == co.Id).ToList();
            }

            return Ok(celestialObject);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects.ToList();

            foreach (CelestialObject co in celestialObjects)
            {
                co.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == co.Id).ToList();
            }

            return Ok(celestialObjects);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new { id = celestialObject.Id }, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var existingCo = _context.CelestialObjects.FirstOrDefault(co => co.Id == id);

            if (existingCo == null)
            {
                return NotFound();
            }

            existingCo.Name = celestialObject.Name;
            existingCo.OrbitalPeriod = celestialObject.OrbitalPeriod;
            existingCo.OrbitedObjectId = celestialObject.OrbitedObjectId;

            _context.CelestialObjects.Update(existingCo);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var existingCo = _context.CelestialObjects.FirstOrDefault(co => co.Id == id);

            if (existingCo == null)
            {
                return NotFound();
            }

            existingCo.Name = name;

            _context.CelestialObjects.Update(existingCo);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var listOfExistingObjects = _context.CelestialObjects.Where(cos => cos.Id == id || cos.OrbitedObjectId == id).ToList();

            if (listOfExistingObjects.Count < 1)
            {
                return NotFound();
            }

            _context.RemoveRange(listOfExistingObjects);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
