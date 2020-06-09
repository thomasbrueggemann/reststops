using System;
using AutoMapper;
using Reststops.Infrastructure.Data;
using Reststops.Core.Interfaces.Repositories;
using System.Threading.Tasks;
using Reststops.Domain.Entities;
using Reststops.Infrastructure.Data.DAO;

namespace Reststops.Infrastructure.Repositories
{
    public class ReststopRepository : IReststopRepository
    {
        private readonly DbContext _dbContext;
        private readonly IMapper _mapper;

        public ReststopRepository(
            DbContext dbContext,
            IMapper mapper
        )
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        #region Public Methods

        public async Task Insert(Reststop reststop)
        {
            ReststopDAO dao = _mapper.Map<ReststopDAO>(reststop);

        }

        #endregion
    }
}
