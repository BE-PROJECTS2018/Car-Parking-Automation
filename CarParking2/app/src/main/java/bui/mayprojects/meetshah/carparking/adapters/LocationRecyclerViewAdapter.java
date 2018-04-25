package bui.mayprojects.meetshah.carparking.adapters;

import android.content.Context;
import android.content.Intent;
import android.support.v7.widget.CardView;
import android.support.v7.widget.RecyclerView;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.TextView;

import com.squareup.picasso.Picasso;

import java.util.List;

import bui.mayprojects.meetshah.carparking.BookingActivity;
import bui.mayprojects.meetshah.carparking.R;
import bui.mayprojects.meetshah.carparking.models.Location;
import butterknife.BindView;
import butterknife.ButterKnife;

/**
 * Created by Meet Shah on 05-04-2018.
 */

public class LocationRecyclerViewAdapter extends RecyclerView.Adapter<LocationRecyclerViewAdapter.ViewHolder> {

    List<Location> locations;
    Context context;


    public LocationRecyclerViewAdapter(Context context, List<Location> locations) {
        this.locations = locations;
        this.context = context;
    }

    private Context getContext() {
        return context;
    }

    @Override
    public ViewHolder onCreateViewHolder(ViewGroup parent, int viewType) {
        View v = LayoutInflater.from(parent.getContext()).inflate(R.layout.parking_lot, parent, false);
        return new ViewHolder(v);
    }

    @Override
    public void onBindViewHolder(ViewHolder holder, int position) {

        Location location = locations.get(position);

        holder.locationName.setText(location.getTitle());
        holder.tvlocationOverview.setText(location.getOverview());

        /* Picasso.with(getContext())
                .load(location.getPosterPath())
                .into(holder.ivLocationImage); */

    }

    @Override
    public int getItemCount() { return locations.size(); }

    public class ViewHolder extends RecyclerView.ViewHolder implements View.OnClickListener {

        @BindView(R.id.locationImage)
        ImageView ivLocationImage;
        @BindView(R.id.locationName)
        TextView locationName;
        @BindView(R.id.locationOverview)
        TextView tvlocationOverview;
        @BindView(R.id.cvParking)
        CardView cvParking;

        ViewHolder(View view) {
            super(view);
            ButterKnife.bind(this, view);
            view.setOnClickListener(this);
        }

        @Override
        public void onClick(View view) {

            Location location = locations.get(getAdapterPosition());

            Intent intent = new Intent(getContext(), BookingActivity.class);
            intent.putExtra("LOCATION", location);
            getContext().startActivity(intent);

        }
    }
}
