using PetAdventure.Gameplay;
using PetAdventure.Pets;
using UnityEngine;

namespace PetAdventure.Core
{
    /// <summary>Dựng bản đồ Greenleaf Village và các nhân vật bằng placeholder sprite.</summary>
    public static class WorldBuilder
    {
        public static readonly Vector2 BoundsMin = new Vector2(-16f, -8.25f);
        public static readonly Vector2 BoundsMax = new Vector2(16f, 8.25f);

        public static void BuildWorld(Transform parent, GameState state)
        {
            GameObject world = new GameObject("World");
            world.transform.SetParent(parent, false);

            CreateGround(world.transform);
            CreateBoundaries(world.transform);
            CreateHouses(world.transform);
            CreateTrees(world.transform);
            CreatePond(world.transform);
            CreateNpcs(world.transform);
            CreateCollectibles(world.transform);
            CreateFences(world.transform);
        }

        public static GameObject CreatePlayer(Transform parent)
        {
            GameObject player = new GameObject("Player");
            player.transform.SetParent(parent, false);
            player.transform.position = new Vector3(-1.75f, -0.5f, 0f);

            Rigidbody2D body = player.AddComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Dynamic;
            body.gravityScale = 0f;
            body.freezeRotation = true;

            BoxCollider2D collider = player.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(0.62f, 0.5f);
            collider.offset = new Vector2(0f, -0.35f);

            player.AddComponent<PlayerMovement>();
            player.AddComponent<PlayerInteractor>();

            GameObject visual = new GameObject("Visual");
            visual.transform.SetParent(player.transform, false);
            SpriteRenderer sprite = visual.AddComponent<SpriteRenderer>();
            sprite.sprite = SpriteFactory.Player();
            sprite.sortingOrder = 1000;
            visual.AddComponent<PlayerVisuals>();

            return player;
        }

        public static PetCompanion CreateCompanion(Transform parent, GameState state, Transform followTarget)
        {
            GameObject pet = new GameObject("PetCompanion");
            pet.transform.SetParent(parent, false);
            pet.transform.position = followTarget.position + new Vector3(-1.2f, -0.35f, 0f);

            SpriteRenderer sprite = pet.AddComponent<SpriteRenderer>();
            sprite.sortingOrder = 900;

            PetCompanion companion = pet.AddComponent<PetCompanion>();
            PetElement element = state.Companion != null ? state.Companion.element : PetElement.Fire;
            companion.Initialize(followTarget, element);
            return companion;
        }

        private static void CreateGround(Transform parent)
        {
            GameObject ground = new GameObject("Ground");
            ground.transform.SetParent(parent, false);
            SpriteRenderer renderer = ground.AddComponent<SpriteRenderer>();
            renderer.sprite = SpriteFactory.Ground();
            renderer.sortingOrder = -5000;
        }

        private static void CreateBoundaries(Transform parent)
        {
            AddWall(parent, new Vector2(0f, 17.5f), new Vector2(64f, 1f));
            AddWall(parent, new Vector2(0f, -17.5f), new Vector2(64f, 1f));
            AddWall(parent, new Vector2(-30.5f, 0f), new Vector2(1f, 36f));
            AddWall(parent, new Vector2(30.5f, 0f), new Vector2(1f, 36f));
        }

        private static void AddWall(Transform parent, Vector2 position, Vector2 size)
        {
            GameObject wall = new GameObject("Wall");
            wall.transform.SetParent(parent, false);
            wall.transform.position = position;

            BoxCollider2D collider = wall.AddComponent<BoxCollider2D>();
            collider.size = size;
        }

        private static void CreateHouses(Transform parent)
        {
            AddProp(parent, "House_A", SpriteFactory.House(0), new Vector2(-11f, 7f));
            AddProp(parent, "House_B", SpriteFactory.House(1), new Vector2(9f, 7f));
            AddProp(parent, "House_C", SpriteFactory.House(2), new Vector2(-12f, -7f));
            AddProp(parent, "House_D", SpriteFactory.House(0), new Vector2(13f, -8f));

            BoxCollider2D colliderA = AddCollider(parent, "House_A", new Vector2(-11f, 6.6f), new Vector2(2.4f, 1.8f));
            colliderA.offset = Vector2.zero;
            AddCollider(parent, "House_B", new Vector2(9f, 6.6f), new Vector2(2.4f, 1.8f));
            AddCollider(parent, "House_C", new Vector2(-12f, -7.4f), new Vector2(2.4f, 1.8f));
            AddCollider(parent, "House_D", new Vector2(13f, -8.4f), new Vector2(2.4f, 1.8f));
        }

        private static void CreateTrees(Transform parent)
        {
            Vector2[] positions =
            {
                new Vector2(-18f, 3f), new Vector2(-16f, -4f), new Vector2(16f, 4f),
                new Vector2(18f, -3f), new Vector2(-6f, 9f), new Vector2(4f, 9f),
                new Vector2(-9f, -11f), new Vector2(8f, -11f)
            };

            foreach (Vector2 position in positions)
            {
                AddProp(parent, "Tree", SpriteFactory.Tree(), position);
                AddCircleCollider(parent, position + new Vector2(0f, -0.7f), 0.35f);
            }
        }

        private static void CreatePond(Transform parent)
        {
            AddCollider(parent, "Pond", new Vector2(17.5f, -7.8f), new Vector2(10f, 6f));
        }

        private static void CreateFences(Transform parent)
        {
            for (int i = 0; i < 6; i++)
            {
                AddProp(parent, "Fence", SpriteFactory.Fence(), new Vector2(-3f + i * 1.5f, 10.6f));
            }
        }

        private static void CreateNpcs(Transform parent)
        {
            AddNpc(parent, "Trưởng làng", PetElement.Earth, new Vector2(-4f, 2f), new[]
            {
                "Chào mừng đến Greenleaf Village!",
                "Bốn nguyên tố, năm linh thú, một hành trình.",
                "Mở Bộ sưu tập rồi chọn 'Đi theo' để linh thú đồng hành."
            });

            AddNpc(parent, "Bác sĩ thú y", PetElement.Water, new Vector2(5f, -2f), new[]
            {
                "Linh thú của bạn cần hồi máu chứ?",
                "Máu tối đa tăng theo cấp: 100 + 50 mỗi cấp."
            });

            AddNpc(parent, "Thương nhân", PetElement.Fire, new Vector2(8f, 3f), new[]
            {
                "Túi đồ của tôi có thức ăn và bóng bắt thú.",
                "Hàng mới sẽ về khi bạn lên cấp."
            });
        }

        private static void AddNpc(Transform parent, string npcName, PetElement element, Vector2 position, string[] lines)
        {
            GameObject npc = new GameObject("NPC_" + npcName);
            npc.transform.SetParent(parent, false);
            npc.transform.position = position;
            npc.transform.localScale = Vector3.one * 1.5f;

            SpriteRenderer renderer = npc.AddComponent<SpriteRenderer>();
            renderer.sprite = SpriteFactory.PetWorld(element);
            renderer.sortingOrder = 800;

            BoxCollider2D collider = npc.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(0.6f, 0.6f);

            NpcInteractable interactable = npc.AddComponent<NpcInteractable>();
            interactable.Configure(npcName, lines);
        }

        private static void CreateCollectibles(Transform parent)
        {
            Vector2[] positions =
            {
                new Vector2(2f, -4f), new Vector2(6f, -6f), new Vector2(-8f, -3f),
                new Vector2(10f, 1f), new Vector2(1f, 6f)
            };

            for (int i = 0; i < positions.Length; i++)
            {
                GameObject item = new GameObject("Collectible_" + i);
                item.transform.SetParent(parent, false);
                item.transform.position = positions[i];
                item.transform.localScale = Vector3.one * 0.7f;

                SpriteRenderer renderer = item.AddComponent<SpriteRenderer>();
                renderer.sprite = SpriteFactory.ItemIcon(i % 2 == 0 ? "herb" : "food");
                renderer.sortingOrder = 700;

                CircleCollider2D collider = item.AddComponent<CircleCollider2D>();
                collider.radius = 0.7f;
                collider.isTrigger = true;

                item.AddComponent<Collectible>();
            }
        }

        private static GameObject AddProp(Transform parent, string name, Sprite sprite, Vector2 position)
        {
            GameObject prop = new GameObject(name);
            prop.transform.SetParent(parent, false);
            prop.transform.position = position;

            SpriteRenderer renderer = prop.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            prop.AddComponent<SortingByY>();

            return prop;
        }

        private static BoxCollider2D AddCollider(Transform parent, string name, Vector2 position, Vector2 size)
        {
            GameObject holder = new GameObject(name + "_Collider");
            holder.transform.SetParent(parent, false);
            holder.transform.position = position;

            BoxCollider2D collider = holder.AddComponent<BoxCollider2D>();
            collider.size = size;
            return collider;
        }

        private static void AddCircleCollider(Transform parent, Vector2 position, float radius)
        {
            GameObject holder = new GameObject("Obstacle");
            holder.transform.SetParent(parent, false);
            holder.transform.position = position;

            CircleCollider2D collider = holder.AddComponent<CircleCollider2D>();
            collider.radius = radius;
        }
    }
}
