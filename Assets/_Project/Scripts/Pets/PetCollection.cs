using System;
using System.Collections.Generic;
using UnityEngine;

namespace PetAdventure.Pets
{
    /// <summary>Danh sách linh thú người chơi sở hữu. Không phụ thuộc UI hay API.</summary>
    public sealed class PetCollection
    {
        private readonly List<PetRuntimeData> pets = new List<PetRuntimeData>();

        public event Action Changed;

        public IReadOnlyList<PetRuntimeData> Pets => pets;

        public int Count => pets.Count;

        public int NextId
        {
            get
            {
                int max = 0;
                foreach (PetRuntimeData pet in pets)
                {
                    if (pet.id > max) max = pet.id;
                }

                return max + 1;
            }
        }

        public PetRuntimeData Get(int id)
        {
            foreach (PetRuntimeData pet in pets)
            {
                if (pet.id == id) return pet;
            }

            return null;
        }

        public PetRuntimeData Add(PetDefinition definition, int? forcedId = null)
        {
            if (definition == null) throw new ArgumentNullException(nameof(definition));

            PetRuntimeData pet = new PetRuntimeData
            {
                id = forcedId ?? NextId,
                name = definition.displayName,
                element = definition.element,
                level = PetStats.MinLevel,
                hp = PetStats.BaseHp,
                description = definition.description,
                petKey = definition.petKey
            };

            pets.Add(pet);
            RaiseChanged();
            return pet;
        }

        public void AddOrReplace(PetRuntimeData pet)
        {
            if (pet == null) return;

            PetRuntimeData existing = Get(pet.id);
            if (existing == null)
            {
                pets.Add(pet);
            }
            else
            {
                existing.name = pet.name;
                existing.element = pet.element;
                existing.level = pet.level;
                existing.hp = pet.hp;
                existing.description = pet.description;
                existing.petKey = pet.petKey;
            }

            RaiseChanged();
        }

        public void ReplaceAll(IEnumerable<PetRuntimeData> incoming)
        {
            pets.Clear();
            if (incoming != null)
            {
                pets.AddRange(incoming);
            }

            RaiseChanged();
        }

        public bool Rename(int id, string newName)
        {
            PetRuntimeData pet = Get(id);
            if (pet == null) return false;

            string trimmed = (newName ?? "").Trim();
            if (trimmed.Length < 2 || trimmed.Length > 50) return false;

            pet.name = trimmed;
            RaiseChanged();
            return true;
        }

        public bool UpdateStats(int id, int level, int hp)
        {
            PetRuntimeData pet = Get(id);
            if (pet == null) return false;

            pet.ApplyLevelAndHp(level, hp);
            RaiseChanged();
            return true;
        }

        public void Clear()
        {
            pets.Clear();
            RaiseChanged();
        }

        public void RaiseChanged()
        {
            Changed?.Invoke();
        }

        /// <summary>Dữ liệu demo cho Giai đoạn 2, xoá khi nối API thật.</summary>
        public void SeedDemo(int count)
        {
            pets.Clear();
            int index = 0;
            foreach (PetElement element in PetElementInfo.All)
            {
                if (index >= count) break;

                PetDefinition definition = PetCatalog.ForElement(element);
                if (definition == null) continue;

                PetRuntimeData pet = new PetRuntimeData
                {
                    id = index + 1,
                    name = definition.displayName,
                    element = definition.element,
                    level = 1 + index * 2,
                    description = definition.description,
                    petKey = definition.petKey
                };
                pet.hp = Mathf.RoundToInt(pet.MaxHp * (0.6f + 0.1f * index));
                pets.Add(pet);
                index++;
            }

            RaiseChanged();
        }
    }
}
