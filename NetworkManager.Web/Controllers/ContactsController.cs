using AutoMapper;
using NetworkManager.Core.Entities;
using NetworkManager.Core.Services;
using NetworkManager.Dto.Dtos;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace NetworkManager.Web.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("api/Contacts")]
    public class ContactsController : ApiController
    {
        private readonly IContactService _contactService;

        public ContactsController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetContacts()
        {
            List<Contact> contacts = await _contactService.GetAllAsync();
            contacts = new List<Contact>();
            for (int i = 0; i <= 100; i++)
            {
                var contact = new Contact();
                contact.Id = i;
                contact.LastName = "Denis"+i*10;
                contact.FirstName = "Makarenko" + i * 10;
                contact.Email = "email@email.com" + i * 10;
                contacts.Add(contact);
            }

            List<ContactDto> contactDtos = new List<ContactDto>();
            
            Mapper.Map(contacts, contactDtos);
            
            return Ok(contactDtos);
        }

        [Route("ById/{id:int}")]
        public async Task<IHttpActionResult> GetContact(int id)
        {
            Contact contact = await _contactService.GetByIdAsync(id);
            if (contact == null)
            {
                contact = new Contact();
                contact.Id = 0;
                contact.LastName = "Denis";
                contact.FirstName = "Makarenko";
                contact.Email = "email@email.com";
               // return Ok(contact);
               // return NotFound();
            }

            ContactDto contactDto = new ContactDto();

            Mapper.Map(contact, contactDto);

            return Ok(contactDto);
        }

        [HttpPut]
        public async Task<IHttpActionResult> PutContact(int id, ContactDto contactDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != contactDto.Id)
            {
                return BadRequest();
            }

            try
            {
                contactDto.Id = id;

                Contact contact = await _contactService.GetByIdAsync(id);

                Mapper.Map(contactDto, contact);

                await _contactService.UpdateAsync(contact);
              
                return Ok(contactDto);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> PostContact(ContactDto contactDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Contact contact = new Contact();
            
            Mapper.Map(contactDto, contact);

            contact.ApplicationUserId = User.Identity.GetUserId();

            contact = await _contactService.AddAsync(contact);
            contactDto.Id = contact.Id;

            return CreatedAtRoute("ApiRoute", new { id = contactDto.Id }, contactDto);
        }

        [HttpDelete]
        public async Task<IHttpActionResult> DeleteContact(int id)
        {
            Contact contact = await _contactService.GetByIdAsync(id);
            if (contact == null)
            {
                return NotFound();
            }

            await _contactService.DeleteAsync(contact);

            return Ok(contact);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _contactService.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ContactExists(int id)
        {
            return _contactService.GetByIdAsync(id) != null;
        }
    }
}