// #define DRAW_AUDIO_GIZMOS // <- get gizmos for changing cone properties, min/max distances in sound emitters (probably only relevant for small scenes).

using Unity.Entities;

namespace Unity.MegaCity.Audio
{
    [WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]
    [UpdateInGroup(typeof(PostBakingSystemGroup))]
    [BakingVersion("Julian", 1)]
    public partial struct SoundEmitterBakingSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Collections.Allocator.Temp);
            foreach (var (soundEmitterBakingData, entity) in SystemAPI.Query<RefRW<SoundEmitterBakingData>>().WithEntityAccess()) 
            {
                var data = soundEmitterBakingData.ValueRO.Authoring.Value;
                SoundEmitter emitter = new SoundEmitter();

                emitter.position = data.transform.position;
                emitter.coneDirection = -data.transform.right;

                if (data.definition != null)
                {
                    emitter.definitionIndex = data.definition.data.definitionIndex;
                    data.definition.Reflect(state.World);
                }

                ecb.AddComponent(entity, emitter);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}
