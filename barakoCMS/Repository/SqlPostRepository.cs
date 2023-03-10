using barakoCMS.Models;
using barakoCMS.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace barakoCMS.Repository {

	public class SqlPostRepository : IPostRepository {
		private readonly AppDbContext _db;

		public SqlPostRepository(AppDbContext db) {
			_db = db ?? throw new ArgumentNullException(nameof(db));
			}

		public async Task AddAsync(Post post) {
			await _db.Posts.AddAsync(post);
			await _db.SaveChangesAsync();
			}

		public async Task DeleteAsync(Post post) {
			var postDb = await _db.Posts.FindAsync(post.Id);
			if (postDb != null) {
				_db.Posts.Remove(postDb);
				await _db.SaveChangesAsync();
				}
			}

		public async Task<IEnumerable<Post>> GetAllAsync(int pageNumber, int pageSize, string searchTerm) {
			var posts = _db.Posts
			.Include(p => p.Author).AsQueryable();

			if (!string.IsNullOrWhiteSpace(searchTerm)) {
				posts = posts.Where(p => p.Content.Contains(searchTerm) || p.Author.UserName.Contains(searchTerm));
				}

			posts = posts.OrderByDescending(p => p.Created)
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize);

			return await posts.ToListAsync();
			}

		public async Task<Post> GetByIdAsync(Guid id) {
			var post = await _db.Posts.FindAsync(id);
			if (post == null) {
				throw new Exception("Post not found.");
				}
			return post;
			}

		public async Task UpdateAsync(Post post) {
			var postDb = await _db.Posts.FindAsync(post.Id);
			if (postDb != null) {
				postDb.Title = post.Title;
				postDb.Content = post.Content;
				postDb.Updated = DateTime.Now;

				await _db.SaveChangesAsync();
				}
			}
		}
	}