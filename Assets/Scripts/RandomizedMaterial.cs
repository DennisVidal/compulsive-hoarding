using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizedMaterial : MonoBehaviour
{
    [Tooltip("Index of the material that will be changed")]
    public int m_MaterialToChangeIndex = 0;

    [Tooltip("Index in Possible Materials that will be used.  -1 -> random")]
    public int m_MaterialToUseIndex = -1;

    [Tooltip("All materials that can be assigned")]
    public List<Material> m_PossibleMaterials;

    protected MeshRenderer m_Renderer;
    void Awake()
    {
        m_Renderer = GetComponent<MeshRenderer>();
        ChangeMaterial(m_MaterialToChangeIndex, m_MaterialToUseIndex);
    }

    private void OnValidate()
    {
        //Keep m_MaterialToChangeIndex in range [0, m_Renderer.sharedMaterials.Length -1]
        if(!m_Renderer)
        {
            return;
        }

        if (m_MaterialToChangeIndex < 0 || m_MaterialToChangeIndex >= m_Renderer.sharedMaterials.Length)
        {
            m_MaterialToChangeIndex = 0;
        }

        //Keep m_MaterialToUseIndex in range [-1, m_PossibleMaterials.Count -1]
        if (m_MaterialToUseIndex < -1 || m_MaterialToUseIndex >= m_PossibleMaterials.Count)
        {
            m_MaterialToUseIndex = -1;
        }
    }

    /// <summary>Changes the material of the object to a certain material in m_PossibleMaterials</summary>
    /// <param name="materialToChangeIndex">Index of the material to change.</param>
    /// <param name="materialToChangeToIndex">Index of the material to change to.</param>
    void ChangeMaterial(int materialToChangeIndex = 0, int materialToChangeToIndex = -1)
    {
        if (!m_Renderer || m_PossibleMaterials.Count == 0)
        {
            return;
        }

        if(materialToChangeToIndex == -1)
        {
            materialToChangeToIndex = Random.Range(0, m_PossibleMaterials.Count);
        }

        //MeshRenderer.sharedMaterial should change the material for all instances.
        //It seems to work the same as just MeshRenderer.material though, while creating less material instances.
        //Might need to be changed to MeshRenderer.material later on.
        List<Material> sharedMaterials = new List<Material>();
        m_Renderer.GetSharedMaterials(sharedMaterials);
        sharedMaterials[materialToChangeIndex] = m_PossibleMaterials[materialToChangeToIndex];
        m_Renderer.sharedMaterials = sharedMaterials.ToArray();
    }
}
