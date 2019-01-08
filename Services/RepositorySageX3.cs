using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VipcoSageX3.Models.SageX3;

namespace VipcoSageX3.Services
{
    public class RepositorySageX3<TEntity> : IRepositorySageX3<TEntity> where TEntity : class
    {
        #region PrivateMembers
        private readonly DbSet<TEntity> Entities;
        private readonly DbQuery<TEntity> Queries;
        private readonly SageX3Context Context;
        private readonly string ErrorMessage = string.Empty;
        #endregion

        #region Constructor
        /// <summary>
        /// The contructor requires an open DataContext to work with
        /// </summary>
        /// <param name="context">An open DataContext</param>

        public RepositorySageX3(SageX3Context context)
        {
            this.Context = context;
            try
            {
                this.Entities = context.Set<TEntity>();
            }
            catch
            {
                this.Queries = context.Query<TEntity>();
            }
        }
        #endregion

        #region Get2
        /// <summary>
        /// Gets the first or default entity based on a predicate, orderby delegate and include delegate. This method default no-tracking query.
        /// </summary>
        /// <param name="selector">The selector for projection.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="orderBy">A function to order elements.</param>
        /// <param name="include">A function to include navigation properties</param>
        /// <param name="disableTracking"><c>True</c> to disable changing tracking; otherwise, <c>false</c>. Default to <c>true</c>.</param>
        /// <returns>An <see cref="IPagedList{TEntity}"/> that contains elements that satisfy the condition specified by <paramref name="predicate"/>.</returns>
        /// <remarks>This method default no-tracking query.</remarks>
        public TResult GetFirstOrDefault<TResult>(Expression<Func<TEntity, TResult>> selector,
                                             Expression<Func<TEntity, bool>> predicate = null,
                                             Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                             Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                             bool disableTracking = true)
        {
            IQueryable<TEntity> query = this.Entities ?? this.Queries.AsQueryable();
            if (disableTracking)
                query = query.AsNoTracking();

            if (include != null)
                query = include(query);

            if (predicate != null)
                query = query.Where(predicate);

            if (orderBy != null)
                return orderBy(query).Select(selector).FirstOrDefault();
            else
                return query.Select(selector).FirstOrDefault();
        }
        /// <summary>
        /// Gets the first or default entity based on a predicate, orderby delegate and include delegate. This method default no-tracking query.
        /// </summary>
        /// <param name="selector">The selector for projection.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="orderBy">A function to order elements.</param>
        /// <param name="include">A function to include navigation properties</param>
        /// <param name="disableTracking"><c>True</c> to disable changing tracking; otherwise, <c>false</c>. Default to <c>true</c>.</param>
        /// <returns>An <see cref="IPagedList{TEntity}"/> that contains elements that satisfy the condition specified by <paramref name="predicate"/>.</returns>
        /// <remarks>This method default no-tracking query.</remarks>
        public async Task<TResult> GetFirstOrDefaultAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
                                     Expression<Func<TEntity, bool>> predicate = null,
                                     Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                     Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                     bool disableTracking = true)
        {
            IQueryable<TEntity> query = this.Entities ?? this.Queries.AsQueryable();
            if (disableTracking)
                query = query.AsNoTracking();

            if (include != null)
                query = include(query);

            if (predicate != null)
                query = query.Where(predicate);

            if (orderBy != null)
                return await orderBy(query).Select(selector).FirstOrDefaultAsync();
            else
                return await query.Select(selector).FirstOrDefaultAsync();
        }
        /// <summary>
        /// Gets the first or default entity based on a predicate, orderby delegate and include delegate. This method default no-tracking query.
        /// </summary>
        /// <param name="selector">The selector for projection.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="orderBy">A function to order elements.</param>
        /// <param name="include">A function to include navigation properties</param>
        /// <param name="skip">How many skip data from database</param>
        /// <param name="take">How many take data from database</param>
        /// <param name="disableTracking"><c>True</c> to disable changing tracking; otherwise, <c>false</c>. Default to <c>true</c>.</param>
        /// <remarks>This method default no-tracking query.</remarks>
        public async Task<ICollection<TResult>> GetToListAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
                                    Expression<Func<TEntity, bool>> predicate = null,
                                    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                    int? skip = null, int? take = null,
                                    bool disableTracking = true)
        {
            IQueryable<TEntity> query = this.Entities ?? this.Queries.AsQueryable();
            if (disableTracking)
                query = query.AsNoTracking();

            if (include != null)
                query = include(query);

            if (predicate != null)
                query = query.Where(predicate);

            if (orderBy != null)
                query = orderBy(query);

            if (skip != null && take != null)
                query = query.Skip(skip.Value).Take(take.Value);

            return await query.Select(selector).ToListAsync();
        }
        /// <summary>
        /// Get length of list
        /// </summary>
        /// <param name="predicate"> A function to test each element for a condition.</param>
        /// <param name="disableTracking"></param>
        /// <returns></returns>
        public async Task<int> GetLengthWithAsync(Expression<Func<TEntity, bool>> predicate = null, bool disableTracking = true)
        {
            IQueryable<TEntity> query = this.Entities ?? this.Queries.AsQueryable();
            if (disableTracking)
                query = query.AsNoTracking();
            if (predicate != null)
                query = query.Where(predicate);

            return await query.CountAsync();
        }

        #endregion

        #region Get

        /// <summary>
        /// Returns a single object with a primary key of the provided id
        /// </summary>
        /// <remarks>Synchronous</remarks>
        /// <param name="id">The primary key of the object to fetch</param>
        /// <returns>A single object with the provided primary key or null</returns>
        public TEntity Get(int id, bool option = false)
        {
            var entity = this.Entities.Find(id);
            if (!option && entity != null)
                this.Context.Entry(entity).State = EntityState.Detached;
            return entity;
        }
        public TEntity Get(string id, bool option = false)
        {
            var entity = this.Entities.Find(id);
            if (!option && entity != null)
                this.Context.Entry(entity).State = EntityState.Detached;
            return entity;
        }
        /// <summary>
        /// Returns a single object with a primary key of the provided id
        /// </summary>
        /// <remarks>Asynchronous</remarks>
        /// <param name="id">The primary key of the object to fetch</param>
        /// <param name="option">Option need lazy relation</param>
        /// <returns>A single object with the provided primary key or null</returns>
        public async Task<TEntity> GetAsync(int id, bool option = false)
        {
            var entity = await Entities.FindAsync(id);
            if (!option && entity != null)
                this.Context.Entry(entity).State = EntityState.Detached;
            return entity;
        }

        public async Task<TEntity> GetAsync(string id, bool option = false)
        {
            var entity = await Entities.FindAsync(id);
            if (!option && entity != null)
                this.Context.Entry(entity).State = EntityState.Detached;
            return entity;
        }
        /// <summary>
        /// Get as IQueryyable
        /// </summary>
        /// <returns></returns>
        public IQueryable<TEntity> GetAllAsQueryable()
        {
            return this.Entities.AsQueryable();
        }
        /// <summary>
        /// Get all entities
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public async Task<ICollection<TEntity>> GetAllAsync(bool option = false)
        {
            var ListData = new List<TEntity>();
            (await this.Entities.ToListAsync()).ForEach(item =>
            {
                if (!option && item != null)
                    this.Context.Entry(item).State = EntityState.Detached;

                ListData.Add(item);
            });
            return ListData;
        }

        public async Task<ICollection<TEntity>> GetAllAsync<TProperty>(Expression<Func<TEntity, TProperty>> include, bool option = false)
        {
            var ListData = new List<TEntity>();
            (await this.Entities.Include(include).ToListAsync()).ForEach(item =>
            {
                if (!option && item != null)
                    this.Context.Entry(item).State = EntityState.Detached;

                ListData.Add(item);
            });
            return ListData;
        }

        /// <summary>
        /// Get all entities with condition
        /// </summary>
        /// <param name="Condition"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public async Task<ICollection<TEntity>> GetAllWithConditionAsync(
          Expression<Func<TEntity, bool>> Condition = null, bool option = false)
        {
            var Query = this.Entities.AsQueryable();

            if (Condition != null)
                Query = Query.Where(Condition);

            var ListData = new List<TEntity>();
            (await this.Entities.ToListAsync()).ForEach(item =>
            {
                if (!option && item != null)
                    this.Context.Entry(item).State = EntityState.Detached;

                ListData.Add(item);
            });
            return ListData;
        }
        #endregion

        #region Find

        /// <summary>
        /// Returns a single object which matches the provided expression
        /// </summary>
        /// <remarks>Synchronous</remarks>
        /// <param name="match">A Linq expression filter to find a single result</param>
        /// <returns>A single object which matches the expression filter.
        /// If more than one object is found or if zero are found, null is returned</returns>
        public TEntity Find(Expression<Func<TEntity, bool>> match, bool option = false)
        {
            var entity = this.Entities.SingleOrDefault(match);
            if (!option && entity != null)
                this.Context.Entry(entity).State = EntityState.Detached;
            return entity;
        }
        /// <summary>
        /// Returns a single object which matches the provided expression
        /// </summary>
        /// <remarks>Asynchronous</remarks>
        /// <param name="match">A Linq expression filter to find a single result</param>
        /// <returns>A single object which matches the expression filter.
        /// If more than one object is found or if zero are found, null is returned</returns>
        public async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> match, bool option = false)
        {
            var entity = await this.Entities.SingleOrDefaultAsync(match);
            if (!option && entity != null)
                this.Context.Entry(entity).State = EntityState.Detached;
            return entity;
        }

        public async Task<TEntity> FindAsync<TProperty>(Expression<Func<TEntity, bool>> match, Expression<Func<TEntity, TProperty>> include, bool option = false)
        {
            var entity = await this.Entities.Include(include).FirstOrDefaultAsync(match);
            if (!option && entity != null)
                this.Context.Entry(entity).State = EntityState.Detached;
            return entity;
        }
        /// <summary>
        /// Returns a collection of objects which match the provided expression
        /// </summary>
        /// <remarks>Synchronous</remarks>
        /// <param name="match">A linq expression filter to find one or more results</param>
        /// <returns>An ICollection of object which match the expression filter</returns>
        public ICollection<TEntity> FindAll(Expression<Func<TEntity, bool>> match, bool option = false)
        {
            var ListData = new List<TEntity>();
            this.Entities.Where(match).ToList().ForEach(item =>
            {
                if (!option && item != null)
                    this.Context.Entry(item).State = EntityState.Detached;
                ListData.Add(item);
            });

            return ListData;
        }
        public async Task<ICollection<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> match, bool option = false)
        {
            var ListData = new List<TEntity>();
            (await this.Entities.Where(match).ToListAsync()).ForEach(item =>
            {
                if (!option && item != null)
                    this.Context.Entry(item).State = EntityState.Detached;
                ListData.Add(item);
            });

            return ListData;
        }

        public async Task<ICollection<TEntity>> FindAllAsync<TProperty>(Expression<Func<TEntity, bool>> match, Expression<Func<TEntity, TProperty>> include, bool option = false)
        {
            var ListData = new List<TEntity>();
            (await this.Entities.Where(match).Include(include).ToListAsync()).ForEach(item =>
            {
                if (!option && item != null)
                    this.Context.Entry(item).State = EntityState.Detached;
                ListData.Add(item);
            });

            return ListData;
        }
        #endregion

        #region Add
        /// <summary>
        /// Inserts a single object to the database and commits the change
        /// </summary>
        /// <remarks>Synchronous</remarks>
        /// <param name="nTEntity">The object to insert</param>
        /// <returns>The resulting object including its primary key after the insert</returns>
        public TEntity Add(TEntity nTEntity)
        {
            this.Entities.Add(nTEntity);
            this.Context.SaveChanges();
            this.Context.Entry(nTEntity).State = EntityState.Deleted;
            return nTEntity;
        }

        /// <summary>
        /// Inserts a single object to the database and commits the change
        /// </summary>
        /// <remarks>Asynchronous</remarks>
        /// <param name="nTEntity">The object to insert</param>
        /// <returns>The resulting object including its primary key after the insert</returns>
        public async Task<TEntity> AddAsync(TEntity nTEntity)
        {
            this.Entities.Add(nTEntity);
            await this.Context.SaveChangesAsync();
            this.Context.Entry(nTEntity).State = EntityState.Detached;
            return nTEntity;
        }
        /// <summary>
        /// Inserts a collection of objects into the database and commits the changes
        /// </summary>
        /// <remarks>Asynchronous</remarks>
        /// <param name="nTEntityList">An IEnumerable list of objects to insert</param>
        /// <returns>The IEnumerable resulting list of inserted objects including the primary keys</returns>
        public async Task<IEnumerable<TEntity>> AddAllAsync(IEnumerable<TEntity> nTEntityList)
        {
            this.Entities.AddRange(nTEntityList);
            await this.Context.SaveChangesAsync();

            foreach (var item in nTEntityList)
                this.Context.Entry(item).State = EntityState.Detached;

            return nTEntityList;
        }
        #endregion

        #region Update

        /// <summary>
        /// Updates a single object based on the provided primary key and commits the change
        /// </summary>
        /// <remarks>Synchronous</remarks>
        /// <param name="updated">The updated object to apply to the database</param>
        /// <param name="key">The primary key of the object to update</param>
        /// <returns>The resulting updated object</returns>
        public TEntity Update(TEntity updated, int key)
        {
            if (updated == null)
                return null;

            TEntity existing = this.Entities.Find(key);
            if (existing != null)
            {
                this.Context.Entry(existing).CurrentValues.SetValues(updated);
                this.Context.SaveChanges();
            }
            return existing;
        }
        public TEntity Update(TEntity updated, string key)
        {
            if (updated == null)
                return null;

            TEntity existing = this.Entities.Find(key);
            if (existing != null)
            {
                this.Context.Entry(existing).CurrentValues.SetValues(updated);
                this.Context.SaveChanges();
            }
            return existing;
        }

        /// <summary>
        /// Updates a single object based on the provided primary key and commits the change
        /// </summary>
        /// <remarks>Asynchronous</remarks>
        /// <param name="updated">The updated object to apply to the database</param>
        /// <param name="key">The primary key of the object to update</param>
        /// <returns>The resulting updated object</returns>
        public async Task<TEntity> UpdateAsync(TEntity updated, int key)
        {
            if (updated == null)
                return null;

            TEntity existing = await this.Entities.FindAsync(key);
            if (existing != null)
            {
                this.Context.Entry(existing).CurrentValues.SetValues(updated);
                await this.Context.SaveChangesAsync();
                this.Context.Entry(existing).State = EntityState.Detached;
            }
            return existing;
        }
        /// <summary>
        /// Updates a single object based on the provided primary key and commits the change
        /// </summary>
        /// <remarks>Asynchronous</remarks>
        /// <param name="updated">The updated object to apply to the database</param>
        /// <param name="key">The primary key of the object to update</param>
        /// <returns>The resulting updated object</returns>
        public async Task<TEntity> UpdateAsync(TEntity updated, string key)
        {
            if (updated == null)
                return null;

            TEntity existing = await this.Entities.FindAsync(key);
            if (existing != null)
            {
                this.Context.Entry(existing).CurrentValues.SetValues(updated);
                await this.Context.SaveChangesAsync();
                this.Context.Entry(existing).State = EntityState.Detached;
            }
            return existing;
        }
        #endregion

        #region Delete
        /// <summary>
        /// Deletes a single object from the database and commits the change
        /// </summary>
        /// <remarks>Synchronous</remarks>
        /// <param name="key">The object to delete</param>
        public void Delete(int key)
        {
            TEntity existing = this.Entities.Find(key);
            if (existing != null)
            {
                this.Entities.Remove(existing);
                this.Context.SaveChanges();
            }
        }
        /// <summary>
        /// Deletes a single object from the database and commits the change
        /// </summary>
        /// <remarks>Synchronous</remarks>
        /// <param name="key">The object to delete</param>
        public void Delete(string key)
        {
            TEntity existing = this.Entities.Find(key);
            if (existing != null)
            {
                this.Entities.Remove(existing);
                this.Context.SaveChanges();
            }
        }
        /// <summary>
        /// Deletes a single object from the database and commits the change
        /// </summary>
        /// <remarks>Asynchronous</remarks>
        /// <param name="key">The primary key of the object to delete</param>
        public async Task<int> DeleteAsync(int key)
        {
            TEntity existing = await this.Entities.FindAsync(key);
            if (existing != null)
            {
                this.Entities.Remove(existing);
                return await this.Context.SaveChangesAsync();
            }
            return 0;
        }
        /// <summary>
        /// Deletes a single object from the database and commits the change
        /// </summary>
        /// <remarks>Asynchronous</remarks>
        /// <param name="key">The primary key of the object to delete</param>
        public async Task<int> DeleteAsync(string key)
        {
            TEntity existing = await this.Entities.FindAsync(key);
            if (existing != null)
            {
                this.Entities.Remove(existing);
                return await this.Context.SaveChangesAsync();
            }
            return 0;
        }
        #endregion

        #region Count
        /// <summary>
        /// Gets the count of the number of objects in the databse
        /// </summary>
        /// <remarks>Synchronous</remarks>
        /// <returns>The count of the number of objects</returns>
        public int Count()
        {
            return this.Context.Set<TEntity>().Count();
        }
        /// <summary>
        /// Gets the count of the number of objects in the database
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public int CountWithMatch(Expression<Func<TEntity, bool>> match)
        {
            return this.Context.Set<TEntity>().Where(match).Count();
        }
        /// <summary>
        /// Gets the count of the number of objects in the databse
        /// </summary>
        /// <remarks>Asynchronous</remarks>
        /// <returns>The count of the number of objects</returns>
        public async Task<int> CountAsync()
        {
            return await this.Context.Set<TEntity>().CountAsync();
        }
        /// <summary>
        /// Gets the count of the number of objects in the databse
        /// </summary>
        /// <remarks>Asynchronous</remarks>
        ///  /// <param name="match">A linq expression filter to find one or more results</param>
        /// <returns>The count of the number of objects</returns>
        public async Task<int> CountWithMatchAsync(Expression<Func<TEntity, bool>> match)
        {
            return await this.Context.Set<TEntity>().Where(match).CountAsync();
        }

        //********************Check Data Have*********************//
        public async Task<bool> AnyDataAsync(Expression<Func<TEntity, bool>> match)
        {
            return await this.Entities.Where(match).AnyAsync();
        }
        #endregion
    }
}
