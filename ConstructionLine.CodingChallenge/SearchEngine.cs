using System.Collections.Generic;
using System.Linq;

namespace ConstructionLine.CodingChallenge
{
    public class SearchEngine
    {
        private readonly List<Shirt> _shirts;

        public SearchEngine(List<Shirt> shirts)
        {
            _shirts = shirts;
        }

        /// <summary>
        /// Perform a search based on the search options provided
        /// </summary>
        /// <param name="options">The options to use when filtering</param>
        /// <returns>
        /// A set of serach results containing counts of the filtered items by size and colour
        /// </returns>
        public SearchResults Search(SearchOptions options)
        {
            List<Shirt> filteredShirts =
                            (from s in _shirts
                             where (!options.Colors.Any() || options.Colors.Any(cl => cl.Id == s.Color.Id)) &&
                                   (!options.Sizes.Any() || options.Sizes.Any(sz => sz.Id == s.Size.Id))
                             select s).ToList();

            List<ColorCount> colorCounts = GetColorCounts(filteredShirts);
            List<SizeCount> sizeCounts = GetSizeCounts(filteredShirts);

            return new SearchResults
            {
                Shirts = filteredShirts,
                ColorCounts = colorCounts,
                SizeCounts = sizeCounts
            };
        }

        /// <summary>
        /// Return the list of group counts for all colors
        /// </summary>
        /// <param name="shirts">The list of shirts to group</param>
        /// <returns>A list of counts by colour, with a default value of 0 if the colour is not found</returns>
        private static List<ColorCount> GetColorCounts(List<Shirt> shirts)
        {
            var colours = from shirt in shirts
                          group shirt by shirt.Color
                            into colorGroup
                          select new ColorCount
                          {
                              Color = colorGroup.Key,
                              Count = colorGroup.Count()
                          };

            return (from color in Color.All
                    join colorGroup in colours
                      on color equals colorGroup.Color
                      into results
                    from result in results.DefaultIfEmpty(new ColorCount
                    {
                        Color = color,
                        Count = 0
                    })
                    select result).ToList();
        }

        /// <summary>
        /// Return the list of group counts for all sizes
        /// </summary>
        /// <param name="shirts">The list of shirts to group</param>
        /// <returns>A list of counts by colour, with a default value of 0 if the size is not found</returns>
        private static List<SizeCount> GetSizeCounts(List<Shirt> shirts)
        {
            var sizes = from shirt in shirts
                        group shirt by shirt.Size
                            into shirtsize
                        select new SizeCount
                        {
                            Size = shirtsize.Key,
                            Count = shirtsize.Count()
                        };

            return (from size in Size.All
                    join sizeGroup in sizes
                     on size equals sizeGroup.Size
                     into results
                    from result in results.DefaultIfEmpty(new SizeCount 
                    { 
                        Size = size, 
                        Count = 0 
                    })
                    select result).ToList();
        }

    }
}