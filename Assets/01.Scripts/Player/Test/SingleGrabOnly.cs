using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SingleGrabOnly : XRGrabInteractable
{
    private IXRSelectInteractor currentInteractor = null;

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        // 한 손만 잡게 하기 위해 현재 interactor 저장
        currentInteractor = args.interactorObject;
        base.OnSelectEntered(args);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        // Grab 해제 시 interactor 초기화
        currentInteractor = null;
        base.OnSelectExited(args);
    }

    public override bool IsSelectableBy(IXRSelectInteractor interactor)
    {
        // 이미 다른 손이 잡고 있으면 무시
        if (currentInteractor != null && currentInteractor != interactor)
            return false;

        return base.IsSelectableBy(interactor);
    }
}