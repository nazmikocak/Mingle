﻿using Mingle.Entities.Enums;

namespace Mingle.Entities.Models
{
    /// <summary>
    /// Grup bilgilerini temsil eden sınıf.
    /// Bir grubun adı, açıklaması, fotoğrafı, katılımcıları ve oluşturulma bilgilerini içerir.
    /// </summary>
    public sealed class Group
    {
        public required string Name { get; set; }

        public required string Description { get; set; }

        public required Uri Photo { get; set; }

        public required Dictionary<string, GroupParticipant> Participants { get; set; }

        public required string CreatedBy { get; set; }

        public required DateTime CreatedDate { get; set; }
    }
}