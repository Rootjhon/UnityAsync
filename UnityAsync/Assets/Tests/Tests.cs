using UnityAsync;
using UnityEngine;
using System.Threading;
using System.Collections;
using System.Diagnostics;
using System.Threading.Tasks;

using UDebug = UnityEngine.Debug;
using WaitForSeconds = UnityAsync.WaitForSeconds;
using WaitForSecondsRealtime = UnityAsync.WaitForSecondsRealtime;
using WaitUntil = UnityAsync.WaitUntil;
using WaitWhile = UnityAsync.WaitWhile;

public sealed class Tests : MonoBehaviour
{
    private Stopwatch sw = new Stopwatch();

    void Start()
    {
        //WaitForFramesTest();
        WaitForSecondsTest();
        //WaitUntilTest();
        //WaitWhileTest();
        //CoroutineAwaiterTest();
        //YieldInstructionAwaiterTest();
        //CustomYieldInstructionAwaiterTest();
        //SyncContextTest();
        //StartCoroutine(TaskYieldTest());
        //ConfigureAwaitTest1();
        //ConfigureAwaitTest2();
        //ConfigureAwaitTest3();

        //PerformanceBaseline();
        //PerformanceTest();
    }

    async void WaitForFramesTest()
    {
        UDebug.Log("WaitForFrames: Should print out \"1\", \"2\", \"3\", \"4\".");

        for (int i = 0; i < 4; ++i)
        {
            UDebug.Log(Time.frameCount);

            await new WaitForFrames(1);
        }

        UDebug.Log("WaitForFrames: Should print out \"25\".");

        await new WaitForFrames(20);

        UDebug.Log(Time.frameCount);
    }

    async void WaitForSecondsTest()
    {
        UDebug.Log("WaitForSeconds: Should print out approx. \"0\", \".5\", \"1\", \"1.5\".");

        // skip first frames (frame time will mess it up otherwise)
        await new WaitForFrames(2);

        // track time from frame 3
        float t = Time.time;

        for (int i = 0; i < 4; ++i)
        {
            UDebug.Log(Time.time - t);
            await new WaitForSeconds(.5f);
        }

        UDebug.Log("WaitForSeconds: Should print out approx. \"1\".");

        Time.timeScale = 0;

        t = Time.unscaledTime;

        await new WaitForSecondsRealtime(1);

        UDebug.Log(Time.unscaledTime - t);
    }

    async void WaitUntilTest()
    {
        UDebug.Log("WaitUntil: Should print out \"Cool Bananas!\" after clicking.");

        await new WaitUntil(() => Input.GetMouseButtonDown(0));

        UDebug.Log("Cool Bananas!");
    }

    async void WaitWhileTest()
    {
        UDebug.Log("WaitWhile: Should print out \"Wowsers, trousers!\" after clicking.");

        await new WaitWhile(() => !Input.GetMouseButtonDown(0));

        UDebug.Log("Wowsers, trousers!");
    }

    async void CoroutineAwaiterTest()
    {
        UDebug.Log("CoroutineAwaiterTest: Should print out \"5\".");

        IEnumerator Blah()
        {
            for (int i = 0; i < 4; ++i)
                yield return null;
        }

        await Blah();

        UDebug.Log(Time.frameCount);
    }

    async void YieldInstructionAwaiterTest()
    {
        await new WaitForFrames(2);

        float t = Time.time;

        await new UnityEngine.WaitForSeconds(1);

        UDebug.Log("YieldInstructionAwaiterTest: Should print out approx. \"1\".");

        UDebug.Log(Time.time - t);
    }

    async void CustomYieldInstructionAwaiterTest()
    {
        await new WaitForFrames(2);

        float t = Time.unscaledTime;

        await new UnityEngine.WaitForSecondsRealtime(1);

        UDebug.Log("CustomYieldInstructionAwaiterTest: Should print out approx. \"1\".");

        UDebug.Log(Time.unscaledTime - t);
    }

    async void SyncContextTest()
    {
        UDebug.Log("SyncContextTest: Should print out \"True\".");

        UDebug.Log(AsyncManager.InUnityContext);

        UDebug.Log("SyncContextTest: Should print out \"False\".");

        await AsyncManager.BackgroundSyncContext;

        UDebug.Log(AsyncManager.InUnityContext);

        UDebug.Log("SyncContextTest: Should print out \"True\".");

        await AsyncManager.UnitySyncContext;

        UDebug.Log(AsyncManager.InUnityContext);
    }

    IEnumerator TaskYieldTest()
    {
        yield return null;
        yield return null;

        float t = Time.time;

        var task = Task.Delay(1000);

        UDebug.Log("TaskYieldTest: Should print out approx. \"1\".");

        yield return task.AsYieldInstruction();

        UDebug.Log(Time.time - t);
    }

    async void ConfigureAwaitTest1()
    {
        var go = new GameObject("Owner");

        async void DestroyOwner()
        {
            await new WaitForSeconds(1);

            Destroy(go);
        }

        UDebug.Log("ConfigureAwaitTest1: Should print out \"Owner destroyed.\" in 1 second.");

        DestroyOwner();

        // set go as owner
        await new WaitWhile(() => true).ConfigureAwait(go);

        UDebug.Log("Owner destroyed.");
    }

    // test inexplicably fails but I believe it is actually working
    async void ConfigureAwaitTest2()
    {
        UDebug.Log(Time.inFixedTimeStep);

        UDebug.Log("ConfigureAwaitTest2: Should print out \"True\".");

        await new WaitForFrames(1).ConfigureAwait(FrameScheduler.FixedUpdate);

        UDebug.Log(Time.inFixedTimeStep);

        UDebug.Log("ConfigureAwaitTest2: Should print out \"False\".");

        await new WaitForFrames(10).ConfigureAwait(FrameScheduler.Update);

        UDebug.Log(Time.inFixedTimeStep);
    }

    // test inexplicably fails but I believe it is actually working
    async void ConfigureAwaitTest3()
    {
        float t = Time.time;

        var tokenSource = new CancellationTokenSource();
        tokenSource.CancelAfter(1000);

        UDebug.Log("ConfigureAwaitTest3: Should print out \"Cancelled\" in 1 second.");

        await new WaitForSeconds(10).ConfigureAwait(tokenSource.Token);

        if (Time.time - t < 10)
            UDebug.Log("Cancelled.");
        else
            UDebug.Log("Finished without cancelling.");
    }

    async void PerformanceTest()
    {
        await Await.Updates(10);

        sw = Stopwatch.StartNew();

        async void routine()
        {
            for (int i = 0; i < 1000; ++i)
                await Await.Seconds(Random.value * .01f);
        }

        for (int i = 0; i < 10000; ++i)
        {
            routine();
        }
    }

    async void PerformanceBaseline()
    {
        await Await.Updates(10);

        sw = Stopwatch.StartNew();

        IEnumerator routine()
        {
            for (int i = 0; i < 1000; ++i)
                yield return new UnityEngine.WaitForSeconds(Random.value * .01f);
        }

        for (int i = 0; i < 10000; ++i)
        {
            StartCoroutine(routine());
        }
    }

    void Update()
    {
        if (Time.frameCount == 500)
        {
            sw.Stop();
            UDebug.Log($"Took {sw.Elapsed.TotalSeconds} seconds.");
        }
    }
}