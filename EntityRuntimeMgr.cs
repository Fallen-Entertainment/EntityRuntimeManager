using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Serialization;
using LiteNetLibManager;
using LiteNetLib;
using Cysharp.Threading.Tasks;
using UnityEditor;

namespace MultiplayerARPG
{

    public class EntityRuntimeMgr : MonoBehaviour
    {
        [Header("EntityRuntimeMgr")]
        public MonsterCharacterEntity Entity;
        public SkinnedMeshRenderer[] BodyMesh;
        public GameObject[] DefaultItems;
        public Transform equipmentObject;

        [Header("Settings")]
        public bool RandomSkinMaterial = true;
        public bool RandomEquipment = true;
        public bool RandomArmorMaterial = true;

        [Header("Indexes")]
        public Material[] SkinMaterials;
        public Material[] EquipmentMaterials;
        public GameObject[] EquipmentObjects;

        [Header("Random Names and Titles")]
        public string[] Names;
        public monsterType MonsterType;
        public string ChampionTitle = "Champion";
        public string MiniBossTitle = "Task Master";
        public string BossTitle = "Overlord";

        // private
        private int equipmentChance;

#if UNITY_EDITOR
        [Header("Set Equipment by EquipmentObjects Children")]
        [InspectorButton(nameof(SetEquipment))]
        public bool setEquipment;
#endif

        public void Start()
        {
            equipmentChance = Entity.Level + 10;
            Randomize();
        }

        public void Randomize()
        {
            RandomizeArmorMaterial();
            RandomizeEquipment();
            RandomizeSkin();
            GetName();
        }

        public void GetName()
        {
            if (monsterType.Normal == MonsterType) return;
            int nameRoll = UnityEngine.Random.Range(0, Names.Length);
            Entity.Title = Names[nameRoll];

            GetTitle();
        }

        public void GetTitle()
        {
            if (monsterType.Boss == MonsterType)
            {
                string name = Entity.Title;
                Entity.Title = BossTitle + " " + name;
            }

            else if (monsterType.MiniBoss == MonsterType)
            {
                string name = Entity.Title;
                Entity.Title = MiniBossTitle + " " + name;
            }

            else
            {
                string name = Entity.Title;
                Entity.Title = ChampionTitle + " " + name;
            }
        }

        public void RandomizeSkin()
        {
            if (RandomSkinMaterial == true)
            {
                // Get Random Skin Value
                int SkinRoll = Random.Range(0, SkinMaterials.Length);

                // Assign Skin Materials
                for (var i = 0; i < SkinMaterials.Length; i++)
                {
                    SkinnedMeshRenderer skinMesh = BodyMesh[i];
                    Material[] skinMats = skinMesh.materials;
                    skinMats[0] = SkinMaterials[SkinRoll];
                    skinMesh.materials = skinMats;
                }
            }
        }

        public void RandomizeEquipment()
        {
            if (RandomArmorMaterial == true && EquipmentObjects.Length > 0 && EquipmentMaterials.Length > 0)
            {
                // Get Equipment Color Roll
                int EquipColorRoll = Random.Range(0, EquipmentMaterials.Length);

                // Set Armor Material
                for (var b = 0; b < EquipmentObjects.Length; b++)
                {
                    SkinnedMeshRenderer armorMesh = EquipmentObjects[b].GetComponent<SkinnedMeshRenderer>();
                    Material[] armorMats = armorMesh.materials;
                    armorMats[0] = EquipmentMaterials[EquipColorRoll];
                    armorMesh.materials = armorMats;
                }
            }
        }

        public void RandomizeArmorMaterial()
        {
            if (RandomEquipment == true && EquipmentObjects.Length > 0)
            {
                // Reset Equipment
                for (var x = 0; x < EquipmentObjects.Length; x++)
                {
                    EquipmentObjects[x].SetActive(false);
                }

                // Set Equipment
                for (var j = 0; j < EquipmentObjects.Length; j++)
                {
                    // Get Equipment Roll
                    int EquipRoll = Random.Range(0, 100);

                    // Compare the Roll to the Equipment Chance
                    if (EquipRoll <= equipmentChance)
                    {
                        EquipmentObjects[j].SetActive(true);

                    }

                    for (var c = 0; c < EquipmentObjects.Length; c++)
                    {
                        for (var d = 0; d < DefaultItems.Length; d++)
                        {
                            if (EquipmentObjects[c] == DefaultItems[d])
                            {
                                EquipmentObjects[c].SetActive(true);
                            }
                        }
                    }
                }
            }
        }


#if UNITY_EDITOR
        [ContextMenu("Set Equipment by transforms Children")]
        public void SetEquipment()
        {
            if (equipmentObject == null || equipmentObject.childCount == 0)
                return;

            List<GameObject> Equipment = new List<GameObject>();
            int containersChildCount = equipmentObject.childCount;
            for (int i = 0; i < containersChildCount; ++i)
            {
                Equipment.Add(equipmentObject.GetChild(i).gameObject);
            }
            this.EquipmentObjects = Equipment.ToArray();
        }
#endif
    }

    [System.Serializable]
    public enum monsterType : byte
    {
        Normal,
        Champion,
        MiniBoss,
        Boss,
    }
}

