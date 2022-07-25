using Types.Shapes;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

namespace AbstractInterfaces
{
	public abstract class Visualization : MonoBehaviour
	{
		private static readonly Hash.ScheduleShapeDelegate[] ShapeJobs =
		{
			ShapeJob<Types.Shapes.Plane>.ScheduleParallel
		  , ShapeJob<Types.Shapes.Sphere>.ScheduleParallel
		  , ShapeJob<Types.Shapes.Torus>.ScheduleParallel
		};

		private static readonly int PositionsId = Shader.PropertyToID("_Positions");
		private static readonly int NormalsId   = Shader.PropertyToID("_Normals");
		private static readonly int ConfigId    = Shader.PropertyToID("_Config");

		[SerializeField]                     private Mesh      instanceMesh;
		[SerializeField]                     private Material  material;
		[SerializeField]                     private ShapeType m_shapeType;
		[SerializeField, Range(0.1f,  10f)]  private float     instanceScale = 2f;
		[SerializeField, Range(1,     512)]  private int       resolution    = 16;
		[SerializeField, Range(-0.5f, 0.5f)] private float     displacement  = 0.1f;

		private NativeArray<float3x4> m_positions;
		private NativeArray<float3x4> m_normals;
		private ComputeBuffer         m_positionsBuffer;
		private ComputeBuffer         m_normalsBuffer;
		private MaterialPropertyBlock m_propertyBlock;
		private bool                  m_isDirty;
		private Bounds                m_bounds;

		private void OnEnable()
		{
			m_isDirty = true;

			int length = resolution * resolution;
			length          = length / 4 + (length & 1);
			m_positions       = new NativeArray<float3x4>(length, Allocator.Persistent);
			m_normals         = new NativeArray<float3x4>(length, Allocator.Persistent);
			m_positionsBuffer = new ComputeBuffer(length * 4, 3 * 4);
			m_normalsBuffer   = new ComputeBuffer(length * 4, 3 * 4);

			m_propertyBlock ??= new MaterialPropertyBlock();
			EnableVisualization(length, m_propertyBlock);
			m_propertyBlock.SetBuffer(PositionsId, m_positionsBuffer);
			m_propertyBlock.SetBuffer(NormalsId,   m_normalsBuffer);
			m_propertyBlock.SetVector(ConfigId, new Vector4(resolution, instanceScale / resolution, displacement));
		}

		private void OnDisable()
		{
			m_positions.Dispose();
			m_normals.Dispose();
			m_positionsBuffer.Release();
			m_normalsBuffer.Release();
			m_positionsBuffer = null;
			m_normalsBuffer   = null;
			DisableVisualization();
		}

		private void OnValidate()
		{
			if (m_positionsBuffer != null && enabled)
			{
				OnDisable();
				OnEnable();
			}
		}

		private void Update()
		{
			if (m_isDirty || transform.hasChanged)
			{
				m_isDirty              = false;
				transform.hasChanged = false;

				UpdateVisualization(m_positions, resolution,
									ShapeJobs[(int) m_shapeType](m_positions, m_normals, resolution, transform.localToWorldMatrix, default));

				m_positionsBuffer.SetData(m_positions.Reinterpret<float3>(3 * 4 * 4));
				m_normalsBuffer.SetData(m_normals.Reinterpret<float3>(3     * 4 * 4));

				m_bounds = new Bounds(transform.position,
									float3(2f * cmax(abs(transform.lossyScale)) + displacement));
			}

			Graphics.DrawMeshInstancedProcedural(instanceMesh, 0, material, m_bounds, resolution * resolution, m_propertyBlock);
		}

		#region Abstract methods

		protected abstract void EnableVisualization(int dataLength, MaterialPropertyBlock propertyBlock);
		protected abstract void DisableVisualization();
		protected abstract void UpdateVisualization(NativeArray<float3x4> positions, int resolution, JobHandle handle);

		#endregion
	}
}