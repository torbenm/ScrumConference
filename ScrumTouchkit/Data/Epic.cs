using Newtonsoft.Json;
using ScrumTouchkit.Controls;
using System.Collections.Generic;

namespace ScrumTouchkit.Data
{
    /// <summary>
    /// Stellt eine Epic dar
    /// </summary>
    public class Epic : ItemBase
    {
        /// <summary>
        /// Liste aller mit dieser Epic verbundenen User Stories
        /// </summary>
        [JsonIgnore]
        public List<UserStory> UserStories
        {
            get;
            set;
        }

        /// <summary>
        /// Initalisiert eine neue Epic
        /// </summary>
        public Epic()
        {
            this.Type = SubType.EPIC;
            this.ItemID = NextEpicID++;
            UserStories = new List<UserStory>();
        }

        /// <summary>
        /// Erstellt eine Darstellung für diese Epic auf der angebenen ScrumSurface
        /// </summary>
        /// <param name="surface"></param>
        /// <returns></returns>
        protected override Controls.Abstract.ItemControl GenerateRepresentation(Controls.ScrumSurface surface)
        {
            return new EpicControl(this, surface);
        }
    }
}
